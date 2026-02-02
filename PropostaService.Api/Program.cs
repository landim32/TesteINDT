using FluentValidation;
using PropostaService.Api.Middlewares;
using PropostaService.Application.Behaviors;
using PropostaService.Application.Mappings;
using PropostaService.Infrastructure.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Proposta Service API", 
        Version = "v1",
        Description = "API para gerenciamento de propostas de seguro"
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAutoMapper(typeof(PropostaMappingProfile));

var applicationAssembly = Assembly.Load("PropostaService.Application");
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(applicationAssembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(applicationAssembly);

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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
