using System.Threading;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
    public interface ICancellableAsyncEventHandler<in TEvent>
        where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
