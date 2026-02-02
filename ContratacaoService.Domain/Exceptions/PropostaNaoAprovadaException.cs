namespace ContratacaoService.Domain.Exceptions;

public class PropostaNaoAprovadaException : DomainException
{
    public PropostaNaoAprovadaException(Guid propostaId) 
        : base($"A proposta {propostaId} não está aprovada e não pode ser contratada")
    {
    }

    public PropostaNaoAprovadaException(string message) : base(message)
    {
    }
}
