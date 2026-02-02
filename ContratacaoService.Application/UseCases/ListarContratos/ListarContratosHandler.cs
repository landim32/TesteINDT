using AutoMapper;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Interfaces.Repositories;
using MediatR;

namespace ContratacaoService.Application.UseCases.ListarContratos;

public class ListarContratosHandler : IRequestHandler<ListarContratosQuery, IEnumerable<ContratoDto>>
{
    private readonly IContratoRepository _contratoRepository;
    private readonly IMapper _mapper;

    public ListarContratosHandler(IContratoRepository contratoRepository, IMapper mapper)
    {
        _contratoRepository = contratoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContratoDto>> Handle(ListarContratosQuery request, CancellationToken cancellationToken)
    {
        var contratos = await _contratoRepository.ListarTodosAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ContratoDto>>(contratos);
    }
}
