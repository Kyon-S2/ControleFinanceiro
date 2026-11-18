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
        BusinessRules.ValidateNome(request.Nome);

        var categoria = new Categoria
        {
            Nome = request.Nome.Trim(),
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

        BusinessRules.ValidateNome(request.Nome);

        categoria.Nome = request.Nome.Trim();
        await db.SaveChangesAsync();

        return new CategoriaResponse(categoria.Id, categoria.Nome);
    }

    public async Task RemoverAsync(Guid usuarioId, int id)
    {
        var categoria = await db.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        db.Categorias.Remove(categoria);
        await db.SaveChangesAsync();
    }
}

