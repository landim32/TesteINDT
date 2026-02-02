using MediatR;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces.Repositories;

namespace PropostaService.Application.UseCases.CriarProposta;

public class CriarPropostaHandler : IRequestHandler<CriarPropostaCommand, Guid>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarPropostaHandler(
        IPropostaRepository propostaRepository,
        IUnitOfWork unitOfWork)
    {
        _propostaRepository = propostaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarPropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = Proposta.Criar(
            request.NomeCliente,
            request.Cpf,
            request.TipoSeguro,
            request.ValorCobertura,
            request.ValorPremio
        );

        await _propostaRepository.AdicionarAsync(proposta, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return proposta.Id;
    }
}
