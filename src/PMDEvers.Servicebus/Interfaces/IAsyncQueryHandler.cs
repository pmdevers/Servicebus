using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Interfaces
{
    public interface IAsyncQueryHandler<in TQuery, TResult> where TQuery : class, IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
