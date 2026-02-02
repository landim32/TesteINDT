namespace ContratacaoService.Application.Saga;

public interface ISagaStep<TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
    Task CompensateAsync(TInput input, CancellationToken cancellationToken = default);
}
