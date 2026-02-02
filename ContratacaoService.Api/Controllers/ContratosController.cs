using ContratacaoService.Api.ViewModels.Request;
using ContratacaoService.Api.ViewModels.Response;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContratacaoService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratosController : ControllerBase
{
    private readonly IContratoApplicationService _contratoService;
    private readonly ILogger<ContratosController> _logger;

    public ContratosController(
        IContratoApplicationService contratoService,
        ILogger<ContratosController> logger)
    {
        _contratoService = contratoService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContratoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarContrato([FromBody] CriarContratoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Criando contrato para proposta {PropostaId}", request.PropostaId);

            var dto = new CriarContratoDto { PropostaId = request.PropostaId };
            var contrato = await _contratoService.CriarContratoAsync(dto, cancellationToken);

            var response = new ContratoResponse
            {
                Id = contrato.Id,
                PropostaId = contrato.PropostaId,
                DataContratacao = contrato.DataContratacao,
                Status = contrato.Status.ToString(),
                DataCriacao = contrato.DataCriacao
            };

            return CreatedAtAction(nameof(ObterContrato), new { id = contrato.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar contrato para proposta {PropostaId}", request.PropostaId);
            return StatusCode(500, new ApiErrorResponse
            {
                Message = "Erro ao criar contrato",
                Details = ex.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContratoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterContrato(Guid id, CancellationToken cancellationToken)
    {
        var contrato = await _contratoService.ObterContratoPorIdAsync(id, cancellationToken);

        if (contrato == null)
            return NotFound();

        var response = new ContratoResponse
        {
            Id = contrato.Id,
            PropostaId = contrato.PropostaId,
            DataContratacao = contrato.DataContratacao,
            Status = contrato.Status.ToString(),
            DataCriacao = contrato.DataCriacao,
            DataAtualizacao = contrato.DataAtualizacao
        };

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContratoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarContratos(CancellationToken cancellationToken)
    {
        var contratos = await _contratoService.ListarContratosAsync(cancellationToken);

        var response = contratos.Select(c => new ContratoResponse
        {
            Id = c.Id,
            PropostaId = c.PropostaId,
            DataContratacao = c.DataContratacao,
            Status = c.Status.ToString(),
            DataCriacao = c.DataCriacao,
            DataAtualizacao = c.DataAtualizacao
        });

        return Ok(response);
    }
}
