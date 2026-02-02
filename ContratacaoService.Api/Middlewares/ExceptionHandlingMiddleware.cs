using ContratacaoService.Api.ViewModels.Response;
using ContratacaoService.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace ContratacaoService.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ocorreu um erro interno no servidor";
        string? details = null;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Erro de validação";
                details = string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage));
                break;

            case PropostaNaoAprovadaException propostaNaoAprovada:
                statusCode = HttpStatusCode.BadRequest;
                message = propostaNaoAprovada.Message;
                break;

            case ContratoInvalidoException contratoInvalido:
                statusCode = HttpStatusCode.BadRequest;
                message = contratoInvalido.Message;
                break;

            case DomainException domainException:
                statusCode = HttpStatusCode.BadRequest;
                message = domainException.Message;
                break;

            default:
                details = exception.Message;
                break;
        }

        var response = new ApiErrorResponse
        {
            Message = message,
            Details = details ?? exception.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}
