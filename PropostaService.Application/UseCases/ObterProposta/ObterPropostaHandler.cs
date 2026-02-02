using AutoMapper;
using MediatR;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Interfaces.Repositories;

namespace PropostaService.Application.UseCases.ObterProposta;

public class ObterPropostaHandler : IRequestHandler<ObterPropostaQuery, PropostaDto?>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IMapper _mapper;

    public ObterPropostaHandler(
        IPropostaRepository propostaRepository,
        IMapper mapper)
    {
        _propostaRepository = propostaRepository;
        _mapper = mapper;
    }

    public async Task<PropostaDto?> Handle(ObterPropostaQuery request, CancellationToken cancellationToken)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (proposta == null)
            return null;

        return _mapper.Map<PropostaDto>(proposta);
    }
}
