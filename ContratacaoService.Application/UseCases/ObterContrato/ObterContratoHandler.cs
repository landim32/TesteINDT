using AutoMapper;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Interfaces.Repositories;
using MediatR;

namespace ContratacaoService.Application.UseCases.ObterContrato;

public class ObterContratoHandler : IRequestHandler<ObterContratoQuery, ContratoDto?>
{
    private readonly IContratoRepository _contratoRepository;
    private readonly IMapper _mapper;

    public ObterContratoHandler(IContratoRepository contratoRepository, IMapper mapper)
    {
        _contratoRepository = contratoRepository;
        _mapper = mapper;
    }

    public async Task<ContratoDto?> Handle(ObterContratoQuery request, CancellationToken cancellationToken)
    {
        var contrato = await _contratoRepository.ObterPorIdAsync(request.ContratoId, cancellationToken);
        return contrato != null ? _mapper.Map<ContratoDto>(contrato) : null;
    }
}
