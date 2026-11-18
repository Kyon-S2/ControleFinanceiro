using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Infrastructure.Auth;
using ControleFinanceiro.Infrastructure.Data;
using ControleFinanceiro.Infrastructure.Repositories;

namespace ControleFinanceiro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IGastoRepository, GastoRepository>();
        services.AddScoped<IGanhoRepository, GanhoRepository>();

        services.AddSingleton<JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGastoService, GastoServiceImpl>();
        services.AddScoped<IGanhoService, GanhoServiceImpl>();

        return services;
    }
}

