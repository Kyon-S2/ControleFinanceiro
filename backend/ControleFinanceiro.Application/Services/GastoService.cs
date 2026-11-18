using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Exceptions;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Services;

public class GastoService(IAppDbContext db) : IGastoService
{
    public async Task<List<GastoResponse>> ObterTodosAsync(Guid usuarioId, string? nome = null, int? categoriaId = null, string? mes = null)
    {
        var query = db.Gastos
            .Include(g => g.Categoria)
            .Where(g => g.UsuarioId == usuarioId);

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            query = query.Where(g => g.Nome.ToLower().Contains(termo));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(g => g.CategoriaId == categoriaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(mes) && DateOnly.TryParseExact(mes.Trim() + "-01", "yyyy-MM-dd", out var date))
        {
            query = query.Where(g => g.Data.Year == date.Year && g.Data.Month == date.Month);
        }

        var list = await query
            .OrderByDescending(g => g.Data)
            .ThenByDescending(g => g.Id)
            .ToListAsync();

        return list.Select(g => new GastoResponse(
            g.Id,
            g.Nome,
            g.Valor,
            g.Data,
            g.CategoriaId,
            g.Categoria?.Nome,
            g.CriadoEm
        )).ToList();
    }

    public async Task<GastoResponse> ObterPorIdAsync(Guid usuarioId, int id)
    {
        var gasto = await db.Gastos
            .Include(g => g.Categoria)
            .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

        if (gasto == null)
            throw new NotFoundException("Gasto não encontrado.");

        return new GastoResponse(
            gasto.Id,
            gasto.Nome,
            gasto.Valor,
            gasto.Data,
            gasto.CategoriaId,
            gasto.Categoria?.Nome,
            gasto.CriadoEm
        );
    }

    public async Task<GastoResponse> CriarAsync(Guid usuarioId, GastoRequest request)
    {
        BusinessRules.ValidateNome(request.Nome);
        BusinessRules.ValidateValor(request.Valor);

        var categoria = await db.Categorias
            .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        var gasto = new Gasto
        {
            Nome = request.Nome.Trim(),
            Valor = request.Valor,
            Data = request.Data,
            CategoriaId = request.CategoriaId,
            UsuarioId = usuarioId,
            CriadoEm = DateTime.UtcNow
        };

        db.Gastos.Add(gasto);
        await db.SaveChangesAsync();

        return new GastoResponse(
            gasto.Id,
            gasto.Nome,
            gasto.Valor,
            gasto.Data,
            gasto.CategoriaId,
            categoria.Nome,
            gasto.CriadoEm
        );
    }

    public async Task<GastoResponse> AtualizarAsync(Guid usuarioId, int id, GastoRequest request)
    {
        var gasto = await db.Gastos
            .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

        if (gasto == null)
            throw new NotFoundException("Gasto não encontrado.");

        BusinessRules.ValidateNome(request.Nome);
        BusinessRules.ValidateValor(request.Valor);

        var categoria = await db.Categorias
            .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        gasto.Nome = request.Nome.Trim();
        gasto.Valor = request.Valor;
        gasto.Data = request.Data;
        gasto.CategoriaId = request.CategoriaId;

        await db.SaveChangesAsync();

        return new GastoResponse(
            gasto.Id,
            gasto.Nome,
            gasto.Valor,
            gasto.Data,
            gasto.CategoriaId,
            categoria.Nome,
            gasto.CriadoEm
        );
    }

    public async Task RemoverAsync(Guid usuarioId, int id)
    {
        var gasto = await db.Gastos
            .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

        if (gasto == null)
            throw new NotFoundException("Gasto não encontrado.");

        db.Gastos.Remove(gasto);
        await db.SaveChangesAsync();
    }
}

