using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SmartCharge.UnitOfWork;

public sealed class UnitOfWorkBehaviour<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await next();
    }
}