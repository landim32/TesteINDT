namespace PropostaService.Domain.Exceptions;

public class PropostaInvalidaException : DomainException
{
    public PropostaInvalidaException(string message) : base(message)
    {
    }

    public PropostaInvalidaException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
