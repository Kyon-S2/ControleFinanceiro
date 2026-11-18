using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Exceptions;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Services;

public class GanhoService(IAppDbContext db) : IGanhoService
{
    public async Task<List<GanhoResponse>> ObterTodosAsync(Guid usuarioId, string? nome = null, int? categoriaId = null, string? mes = null)
    {
        var query = db.Ganhos
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

        return list.Select(g => new GanhoResponse(
            g.Id,
            g.Nome,
            g.Valor,
            g.Data,
            g.CategoriaId,
            g.Categoria?.Nome,
            g.Fixo,
            g.CriadoEm
        )).ToList();
    }

    public async Task<GanhoResponse> ObterPorIdAsync(Guid usuarioId, int id)
    {
        var ganho = await db.Ganhos
            .Include(g => g.Categoria)
            .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

        if (ganho == null)
            throw new NotFoundException("Ganho não encontrado.");

        return new GanhoResponse(
            ganho.Id,
            ganho.Nome,
            ganho.Valor,
            ganho.Data,
            ganho.CategoriaId,
            ganho.Categoria?.Nome,
            ganho.Fixo,
            ganho.CriadoEm
        );
    }

    public async Task<GanhoResponse> CriarAsync(Guid usuarioId, GanhoRequest request)
    {
        BusinessRules.ValidateNome(request.Nome);
        BusinessRules.ValidateValor(request.Valor);

        var categoria = await db.Categorias
            .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        var ganho = new Ganho
        {
            Nome = request.Nome.Trim(),
            Valor = request.Valor,
            Data = request.Data,
            CategoriaId = request.CategoriaId,
            UsuarioId = usuarioId,
            Fixo = request.Fixo,
            CriadoEm = DateTime.UtcNow
        };

        db.Ganhos.Add(ganho);
        await db.SaveChangesAsync();

        return new GanhoResponse(
            ganho.Id,
            ganho.Nome,
            ganho.Valor,
            ganho.Data,
            ganho.CategoriaId,
            categoria.Nome,
            ganho.Fixo,
            ganho.CriadoEm
        );
    }

    public async Task<GanhoResponse> AtualizarAsync(Guid usuarioId, int id, GanhoRequest request)
    {
        var ganho = await db.Ganhos
            .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

        if (ganho == null)
            throw new NotFoundException("Ganho não encontrado.");

        BusinessRules.ValidateNome(request.Nome);
        BusinessRules.ValidateValor(request.Valor);

        var categoria = await db.Categorias
            .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.UsuarioId == usuarioId);

        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");

        ganho.Nome = request.Nome.Trim();
        ganho.Valor = request.Valor;
        ganho.Data = request.Data;
        ganho.CategoriaId = request.CategoriaId;
        ganho.Fixo = request.Fixo;

        await db.SaveChangesAsync();

        return new GanhoResponse(
            ganho.Id,
            ganho.Nome,
            ganho.Valor,
            ganho.Data,
            ganho.CategoriaId,
            categoria.Nome,
            ganho.Fixo,
            ganho.CriadoEm
        );
    }

    public async Task RemoverAsync(Guid usuarioId, int id)
    {
        var ganho = await db.Ganhos
            .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

        if (ganho == null)
            throw new NotFoundException("Ganho não encontrado.");

        db.Ganhos.Remove(ganho);
        await db.SaveChangesAsync();
    }
}

