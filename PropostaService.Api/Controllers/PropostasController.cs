using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropostaService.Api.ViewModels.Request;
using PropostaService.Api.ViewModels.Response;
using PropostaService.Application.DTOs;
using PropostaService.Application.UseCases.AlterarStatusProposta;
using PropostaService.Application.UseCases.AtualizarProposta;
using PropostaService.Application.UseCases.CriarProposta;
using PropostaService.Application.UseCases.ListarPropostas;
using PropostaService.Application.UseCases.ObterProposta;

namespace PropostaService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropostasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PropostasController> _logger;

    public PropostasController(IMediator mediator, ILogger<PropostasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova proposta de seguro
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarProposta([FromBody] CriarPropostaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando nova proposta para cliente: {NomeCliente}", request.NomeCliente);

        var command = new CriarPropostaCommand(
            request.NomeCliente,
            request.Cpf,
            request.TipoSeguro,
            request.ValorCobertura,
            request.ValorPremio
        );

        var propostaId = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Proposta criada com sucesso. ID: {PropostaId}", propostaId);

        return CreatedAtAction(nameof(ObterProposta), new { id = propostaId }, propostaId);
    }

    /// <summary>
    /// Obtém uma proposta de seguro por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PropostaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterProposta(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo proposta: {PropostaId}", id);

        var query = new ObterPropostaQuery(id);
        var proposta = await _mediator.Send(query, cancellationToken);

        if (proposta == null)
        {
            _logger.LogWarning("Proposta não encontrada: {PropostaId}", id);
            return NotFound();
        }

        var response = MapToResponse(proposta);
        return Ok(response);
    }

    /// <summary>
    /// Lista todas as propostas de seguro
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PropostaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPropostas(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando todas as propostas");

        var query = new ListarPropostasQuery();
        var propostas = await _mediator.Send(query, cancellationToken);

        var response = propostas.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Atualiza uma proposta de seguro
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarProposta(Guid id, [FromBody] AtualizarPropostaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando proposta: {PropostaId}", id);

        var command = new AtualizarPropostaCommand(
            id,
            request.NomeCliente,
            request.ValorCobertura,
            request.ValorPremio
        );

        await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Proposta atualizada com sucesso: {PropostaId}", id);

        return NoContent();
    }

    /// <summary>
    /// Altera o status de uma proposta de seguro
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarStatus(Guid id, [FromBody] AlterarStatusPropostaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Alterando status da proposta: {PropostaId} para {NovoStatus}", id, request.Status);

        var command = new AlterarStatusPropostaCommand(id, request.Status);
        await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Status da proposta alterado com sucesso: {PropostaId}", id);

        return NoContent();
    }

    private static PropostaResponse MapToResponse(PropostaDto dto)
    {
        return new PropostaResponse
        {
            Id = dto.Id,
            NomeCliente = dto.NomeCliente,
            Cpf = dto.Cpf,
            TipoSeguro = dto.TipoSeguro,
            ValorCobertura = dto.ValorCobertura,
            ValorPremio = dto.ValorPremio,
            DataCriacao = dto.DataCriacao,
            Status = dto.Status
        };
    }
}
