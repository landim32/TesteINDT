using FluentValidation;
using PropostaService.Api.ViewModels.Response;
using PropostaService.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace PropostaService.Api.Middlewares;

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
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = exception switch
        {
            ValidationException validationException => new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed",
                Errors = validationException.Errors.Select(e => e.ErrorMessage)
            },
            PropostaInvalidaException propostaInvalidaException => new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = propostaInvalidaException.Message
            },
            DomainException domainException => new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = domainException.Message
            },
            _ => new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An internal server error occurred",
                Details = exception.Message
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
