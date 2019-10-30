namespace PMDEvers.Servicebus
{
    public interface IEventHandler<in TEvent>
        where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
