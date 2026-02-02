using MediatR;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Interfaces.Repositories;

namespace PropostaService.Application.UseCases.AtualizarProposta;

public class AtualizarPropostaHandler : IRequestHandler<AtualizarPropostaCommand, Unit>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarPropostaHandler(
        IPropostaRepository propostaRepository,
        IUnitOfWork unitOfWork)
    {
        _propostaRepository = propostaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AtualizarPropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(request.PropostaId, cancellationToken);

        if (proposta == null)
            throw new PropostaInvalidaException($"Proposta com ID {request.PropostaId} não encontrada");

        if (!string.IsNullOrWhiteSpace(request.NomeCliente))
        {
            proposta.AtualizarCliente(request.NomeCliente);
        }

        if (request.ValorCobertura.HasValue && request.ValorPremio.HasValue)
        {
            proposta.AtualizarSeguro(request.ValorCobertura.Value, request.ValorPremio.Value);
        }

        await _propostaRepository.AtualizarAsync(proposta, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
