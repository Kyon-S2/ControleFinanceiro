using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.Services;

public class ResumoService(IAppDbContext db) : IResumoService
{
    public async Task<ResumoResponse> ObterResumoAsync(Guid usuarioId, string? mes = null)
    {
        int ano;
        int mesNum;

        if (!string.IsNullOrWhiteSpace(mes) && DateOnly.TryParseExact(mes.Trim() + "-01", "yyyy-MM-dd", out var parsedDate))
        {
            ano = parsedDate.Year;
            mesNum = parsedDate.Month;
        }
        else
        {
            var hoje = DateTime.UtcNow;
            ano = hoje.Year;
            mesNum = hoje.Month;
        }

        var gastosDoMes = await db.Gastos
            .Where(g => g.UsuarioId == usuarioId && g.Data.Year == ano && g.Data.Month == mesNum)
            .Select(g => g.Valor)
            .ToListAsync();

        var totalGastos = gastosDoMes.Sum();
        var qtdGastos = gastosDoMes.Count;
        var maiorGasto = gastosDoMes.Count > 0 ? gastosDoMes.Max() : 0m;

        var ganhosDoMes = await db.Ganhos
            .Where(g => g.UsuarioId == usuarioId && g.Data.Year == ano && g.Data.Month == mesNum)
            .Select(g => g.Valor)
            .ToListAsync();

        var totalGanhos = ganhosDoMes.Sum();
        var saldo = totalGanhos - totalGastos;

        var limiteEntidade = await db.LimitesMensais
            .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);
        var limite = limiteEntidade?.Valor ?? 0m;

        decimal percentualLimite = 0m;
        string statusLimite = "Ok";

        if (limite > 0)
        {
            percentualLimite = Math.Round((totalGastos / limite) * 100m, 2);
            if (percentualLimite >= 100m)
                statusLimite = "Estourado";
            else if (percentualLimite >= 80m)
                statusLimite = "Alerta80";
            else
                statusLimite = "Ok";
        }

        return new ResumoResponse(
            totalGastos,
            qtdGastos,
            maiorGasto,
            totalGanhos,
            saldo,
            limite,
            percentualLimite,
            statusLimite
        );
    }
}

