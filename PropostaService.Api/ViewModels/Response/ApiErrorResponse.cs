namespace PropostaService.Api.ViewModels.Response;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
