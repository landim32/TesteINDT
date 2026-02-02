using MediatR;
using PropostaService.Domain.Interfaces.Repositories;

namespace PropostaService.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (IsQuery(request))
            return await next();

        try
        {
            var response = await next();
            return response;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsQuery(TRequest request)
    {
        return request.GetType().Name.EndsWith("Query");
    }
}
