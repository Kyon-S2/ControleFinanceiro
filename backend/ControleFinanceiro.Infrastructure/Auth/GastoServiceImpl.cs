using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Interfaces;

namespace ControleFinanceiro.Infrastructure.Auth;

public class GastoServiceImpl(IGastoRepository repo) : IGastoService
{
    public async Task<List<GastoResponse>> ObterTodosAsync(int usuarioId)
    {
        var gastos = await repo.ObterTodosPorUsuarioAsync(usuarioId);
        return gastos.Select(g => new GastoResponse(g.Id, g.Nome, g.Valor, g.Classe, g.Data)).ToList();
    }

    public async Task<GastoResponse> CriarAsync(int usuarioId, GastoRequest request)
    {
        var gasto = new Gasto
        {
            Nome = request.Nome,
            Valor = request.Valor,
            Classe = request.Classe,
            Data = request.Data,
            UsuarioId = usuarioId
        };
        await repo.CriarAsync(gasto);
        return new GastoResponse(gasto.Id, gasto.Nome, gasto.Valor, gasto.Classe, gasto.Data);
    }

    public async Task<GastoResponse> AtualizarAsync(int usuarioId, int id, GastoRequest request)
    {
        var gasto = await repo.ObterPorIdAsync(id, usuarioId)
            ?? throw new KeyNotFoundException("Gasto não encontrado.");
        gasto.Nome = request.Nome;
        gasto.Valor = request.Valor;
        gasto.Classe = request.Classe;
        gasto.Data = request.Data;
        await repo.AtualizarAsync(gasto);
        return new GastoResponse(gasto.Id, gasto.Nome, gasto.Valor, gasto.Classe, gasto.Data);
    }

    public async Task RemoverAsync(int usuarioId, int id)
    {
        var gasto = await repo.ObterPorIdAsync(id, usuarioId)
            ?? throw new KeyNotFoundException("Gasto não encontrado.");
        await repo.RemoverAsync(gasto);
    }
}

