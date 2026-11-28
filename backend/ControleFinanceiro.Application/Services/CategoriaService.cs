using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Exceptions;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Services;

public class CategoriaService(IAppDbContext db) : ICategoriaService
{
    public async Task<List<CategoriaResponse>> ObterTodasAsync(Guid usuarioId)
    {
        return await db.Categorias
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaResponse(c.Id, c.Nome))
            .ToListAsync();
    }

    public async Task<CategoriaResponse> ObterPorIdAsync(Guid usuarioId, int id)
    {
        var categoria = await db.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        return new CategoriaResponse(categoria.Id, categoria.Nome);
    }

    public async Task<CategoriaResponse> CriarAsync(Guid usuarioId, CategoriaRequest request)
    {
        BusinessRules.ValidateCategoriaNome(request.Nome);

        var nomeTrimmed = request.Nome.Trim();
        var duplicada = await db.Categorias
            .AnyAsync(c => c.UsuarioId == usuarioId && c.Nome.ToLower() == nomeTrimmed.ToLower());
        if (duplicada)
            throw new ConflitoException("Já existe uma categoria com este nome.");

        var categoria = new Categoria
        {
            Nome = nomeTrimmed,
            UsuarioId = usuarioId
        };

        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();

        return new CategoriaResponse(categoria.Id, categoria.Nome);
    }

    public async Task<CategoriaResponse> AtualizarAsync(Guid usuarioId, int id, CategoriaRequest request)
    {
        var categoria = await db.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        BusinessRules.ValidateCategoriaNome(request.Nome);

        var nomeTrimmed = request.Nome.Trim();
        var duplicada = await db.Categorias
            .AnyAsync(c => c.UsuarioId == usuarioId && c.Id != id && c.Nome.ToLower() == nomeTrimmed.ToLower());
        if (duplicada)
            throw new ConflitoException("Já existe uma categoria com este nome.");

        categoria.Nome = nomeTrimmed;
        await db.SaveChangesAsync();

        return new CategoriaResponse(categoria.Id, categoria.Nome);
    }

    public async Task RemoverAsync(Guid usuarioId, int id)
    {
        var categoria = await db.Categorias
            .Include(c => c.Gastos)
            .Include(c => c.Ganhos)
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        if (categoria.Gastos.Count > 0 || categoria.Ganhos.Count > 0)
            throw new ConflitoException(
                "Não é possível excluir esta categoria pois ela possui lançamentos vinculados. " +
                "Remova ou reclassifique os lançamentos antes de excluir.");

        db.Categorias.Remove(categoria);
        await db.SaveChangesAsync();
    }
}

