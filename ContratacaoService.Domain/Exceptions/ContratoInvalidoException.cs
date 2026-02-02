namespace ContratacaoService.Domain.Exceptions;

public class ContratoInvalidoException : DomainException
{
    public ContratoInvalidoException(string message) : base(message)
    {
    }

    public ContratoInvalidoException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
