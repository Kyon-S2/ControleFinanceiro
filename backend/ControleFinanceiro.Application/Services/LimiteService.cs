using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Exceptions;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Services;

public class LimiteService(IAppDbContext db) : ILimiteService
{
    public async Task<LimiteResponse> ObterAsync(Guid usuarioId)
    {
        var limite = await db.LimitesMensais
            .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);

        return new LimiteResponse(limite?.Valor ?? 0m);
    }

    public async Task<LimiteResponse> DefinirAsync(Guid usuarioId, LimiteRequest request)
    {
        if (request.Valor < 0)
            throw new ValidationException("O valor do limite não pode ser negativo.");

        var limite = await db.LimitesMensais
            .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);

        if (limite == null)
        {
            limite = new LimiteMensal
            {
                UsuarioId = usuarioId,
                Valor = request.Valor
            };
            db.LimitesMensais.Add(limite);
        }
        else
        {
            limite.Valor = request.Valor;
        }

        await db.SaveChangesAsync();

        return new LimiteResponse(limite.Valor);
    }
}

