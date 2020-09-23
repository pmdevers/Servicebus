using System.Threading;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Interfaces
{
    public interface ICancellableAsyncQueryHandler<in TQuery, TResult> where TQuery : class, IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
