using AutoMapper;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Interfaces;
using ContratacaoService.Application.UseCases.CriarContrato;
using ContratacaoService.Application.UseCases.ListarContratos;
using ContratacaoService.Application.UseCases.ObterContrato;
using MediatR;

namespace ContratacaoService.Application.Services;

public class ContratoApplicationService : IContratoApplicationService
{
    private readonly IMediator _mediator;

    public ContratoApplicationService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ContratoDto> CriarContratoAsync(CriarContratoDto dto, CancellationToken cancellationToken = default)
    {
        var command = new CriarContratoCommand(dto.PropostaId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<ContratoDto?> ObterContratoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new ObterContratoQuery(id);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<ContratoDto>> ListarContratosAsync(CancellationToken cancellationToken = default)
    {
        var query = new ListarContratosQuery();
        return await _mediator.Send(query, cancellationToken);
    }
}
