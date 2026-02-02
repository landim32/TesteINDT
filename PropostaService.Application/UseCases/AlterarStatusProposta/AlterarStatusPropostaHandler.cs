using MediatR;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Interfaces.Repositories;

namespace PropostaService.Application.UseCases.AlterarStatusProposta;

public class AlterarStatusPropostaHandler : IRequestHandler<AlterarStatusPropostaCommand, Unit>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AlterarStatusPropostaHandler(
        IPropostaRepository propostaRepository,
        IUnitOfWork unitOfWork)
    {
        _propostaRepository = propostaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AlterarStatusPropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(request.PropostaId, cancellationToken);

        if (proposta == null)
            throw new PropostaInvalidaException($"Proposta com ID {request.PropostaId} não encontrada");

        proposta.AlterarStatus(request.NovoStatus);

        await _propostaRepository.AtualizarAsync(proposta, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
