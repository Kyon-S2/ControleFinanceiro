using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Interfaces;

namespace ControleFinanceiro.Infrastructure.Auth;

public class GanhoServiceImpl(IGanhoRepository repo) : IGanhoService
{
    public async Task<List<GanhoResponse>> ObterTodosAsync(int usuarioId)
    {
        var ganhos = await repo.ObterTodosPorUsuarioAsync(usuarioId);
        return ganhos.Select(g => new GanhoResponse(g.Id, g.Nome, g.Valor, g.Classe, g.Data, g.Fixo)).ToList();
    }

    public async Task<GanhoResponse> CriarAsync(int usuarioId, GanhoRequest request)
    {
        var ganho = new Ganho
        {
            Nome = request.Nome,
            Valor = request.Valor,
            Classe = request.Classe,
            Data = request.Data,
            Fixo = request.Fixo,
            UsuarioId = usuarioId
        };
        await repo.CriarAsync(ganho);
        return new GanhoResponse(ganho.Id, ganho.Nome, ganho.Valor, ganho.Classe, ganho.Data, ganho.Fixo);
    }

    public async Task RemoverAsync(int usuarioId, int id)
    {
        var ganho = await repo.ObterPorIdAsync(id, usuarioId)
            ?? throw new KeyNotFoundException("Ganho não encontrado.");
        await repo.RemoverAsync(ganho);
    }
}

