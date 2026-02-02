using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Interfaces;
using MediatR;

namespace ContratacaoService.Application.UseCases.VerificarProposta;

public class VerificarPropostaHandler : IRequestHandler<VerificarPropostaQuery, PropostaDto?>
{
    private readonly IPropostaServiceClient _propostaServiceClient;

    public VerificarPropostaHandler(IPropostaServiceClient propostaServiceClient)
    {
        _propostaServiceClient = propostaServiceClient;
    }

    public async Task<PropostaDto?> Handle(VerificarPropostaQuery request, CancellationToken cancellationToken)
    {
        return await _propostaServiceClient.ObterPropostaAsync(request.PropostaId, cancellationToken);
    }
}
