namespace ControleFinanceiro.Domain.Interfaces;

using ControleFinanceiro.Domain.Entities;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario> CriarAsync(Usuario usuario);
}

