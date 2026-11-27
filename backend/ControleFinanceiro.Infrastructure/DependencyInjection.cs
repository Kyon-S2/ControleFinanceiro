using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.Infrastructure.Auth;
using ControleFinanceiro.Infrastructure.Data;

namespace ControleFinanceiro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(connectionString));
        }

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IGastoService, GastoService>();
        services.AddScoped<IGanhoService, GanhoService>();
        services.AddScoped<ILimiteService, LimiteService>();
        services.AddScoped<IResumoService, ResumoService>();

        return services;
    }
}
