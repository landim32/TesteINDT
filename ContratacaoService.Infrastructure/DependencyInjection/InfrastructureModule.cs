using ContratacaoService.Application.Behaviors;
using ContratacaoService.Application.Interfaces;
using ContratacaoService.Application.Mappings;
using ContratacaoService.Application.Saga;
using ContratacaoService.Application.Saga.SagaSteps;
using ContratacaoService.Application.Services;
using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Domain.Interfaces.Services;
using ContratacaoService.Domain.Services;
using ContratacaoService.Infrastructure.EventHandlers;
using ContratacaoService.Infrastructure.ExternalServices.HttpClients;
using ContratacaoService.Infrastructure.ExternalServices.Messaging;
using ContratacaoService.Infrastructure.Persistence.Context;
using ContratacaoService.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace ContratacaoService.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddDomainServices();
        services.AddApplicationServices();
        services.AddMediatRServices();
        services.AddAutoMapperServices();
        services.AddExternalServices(configuration);
        services.AddMessaging(configuration);

        return services;
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ContratacaoDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IContratoRepository, ContratoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IContratoValidationService, ContratoValidationService>();
        services.AddScoped<IPropostaValidationService, PropostaValidationService>();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IContratoApplicationService, ContratoApplicationService>();
        
        services.AddScoped<VerificarPropostaStep>();
        services.AddScoped<CriarContratoStep>();
        services.AddScoped<NotificarContratoStep>();
        services.AddScoped<ContratacaoSaga>();
    }

    private static void AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ContratoApplicationService).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(ContratoCriadoEventHandler).Assembly);
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        services.AddValidatorsFromAssemblyContaining<ContratoApplicationService>();
    }

    private static void AddAutoMapperServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ContratoMappingProfile).Assembly);
    }

    private static void AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var propostaServiceUrl = configuration["ExternalServices:PropostaService:BaseUrl"] 
            ?? "http://localhost:5000";

        services.AddHttpClient<IPropostaServiceClient, PropostaServiceClient>(client =>
        {
            client.BaseAddress = new Uri(propostaServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }

    private static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqPort = int.Parse(configuration["RabbitMQ:Port"] ?? "5672");
        var rabbitMqUser = configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPassword = configuration["RabbitMQ:Password"] ?? "guest";

        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqHost,
                Port = rabbitMqPort,
                UserName = rabbitMqUser,
                Password = rabbitMqPassword,
                DispatchConsumersAsync = true
            };
            return factory.CreateConnection();
        });

        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
        services.AddScoped<IMessageConsumer, RabbitMqConsumer>();
        services.AddScoped<RabbitMqInitializer>();
    }
}
