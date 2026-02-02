using ContratacaoService.Api.Middlewares;
using ContratacaoService.Infrastructure.DependencyInjection;
using ContratacaoService.Infrastructure.ExternalServices.Messaging;
using ContratacaoService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Contratacao Service API", 
        Version = "v1",
        Description = "API para gerenciamento de contratos de seguro"
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContratacaoDbContext>();
    await dbContext.Database.MigrateAsync();
    
    // Inicializar RabbitMQ (exchanges e queues)
    var rabbitMqInitializer = scope.ServiceProvider.GetRequiredService<RabbitMqInitializer>();
    rabbitMqInitializer.Initialize();
}

app.Run();
