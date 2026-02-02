using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Interfaces;
using ContratacaoService.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ContratacaoService.Infrastructure.ExternalServices.HttpClients;

public class PropostaServiceClient : IPropostaServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PropostaServiceClient> _logger;

    public PropostaServiceClient(HttpClient httpClient, ILogger<PropostaServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PropostaDto?> ObterPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Consultando proposta {PropostaId} no PropostaService", propostaId);

            var response = await _httpClient.GetAsync($"api/propostas/{propostaId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Proposta {PropostaId} não encontrada. Status: {StatusCode}", propostaId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var proposta = JsonSerializer.Deserialize<PropostaDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Proposta {PropostaId} obtida com sucesso", propostaId);
            return proposta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar proposta {PropostaId}", propostaId);
            throw;
        }
    }

    public async Task<bool> VerificarPropostaAprovadaAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        var proposta = await ObterPropostaAsync(propostaId, cancellationToken);
        return proposta?.Status == StatusProposta.Aprovada;
    }
}
