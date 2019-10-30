using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
    public interface IAsyncEventHandler<in TEvent>
        where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
