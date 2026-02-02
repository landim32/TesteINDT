using AutoMapper;
using MediatR;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Interfaces.Repositories;

namespace PropostaService.Application.UseCases.ListarPropostas;

public class ListarPropostasHandler : IRequestHandler<ListarPropostasQuery, IEnumerable<PropostaDto>>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IMapper _mapper;

    public ListarPropostasHandler(
        IPropostaRepository propostaRepository,
        IMapper mapper)
    {
        _propostaRepository = propostaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PropostaDto>> Handle(ListarPropostasQuery request, CancellationToken cancellationToken)
    {
        var propostas = await _propostaRepository.ObterTodasAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PropostaDto>>(propostas);
    }
}
