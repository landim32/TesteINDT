using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Domain.Interfaces.Repositories;
using PropostaService.Domain.Interfaces.Services;
using PropostaService.Domain.Services;
using PropostaService.Infrastructure.Persistence.Context;
using PropostaService.Infrastructure.Persistence.Repositories;

namespace PropostaService.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PropostaDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(PropostaDbContext).Assembly.FullName)));

        services.AddScoped<IPropostaRepository, PropostaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPropostaValidationService, PropostaValidationService>();

        return services;
    }
}
