using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Internal
{
    internal abstract class EventHandler
    {
        public abstract Task HandleAsync(IEvent @event, CancellationToken cancellationToken,
            MultiInstanceFactory multiInstanceFactory,
            Func<IEnumerable<Task>, Task> publish);
    }

    internal class EventHandler<TEvent> : EventHandler
        where TEvent : IEvent
    {
        public override Task HandleAsync(IEvent @event, CancellationToken cancellationToken,
            MultiInstanceFactory multiInstanceFactory, Func<IEnumerable<Task>, Task> publish)
        {
            var handlers = GetHandlers((TEvent)@event, cancellationToken, multiInstanceFactory);
            return publish(handlers);
        }

        private static IEnumerable<THandler> GetHandlers<THandler>(MultiInstanceFactory factory)
        {
            return factory(typeof(THandler)).Cast<THandler>();
        }

        private IEnumerable<Task> GetHandlers(TEvent @event, CancellationToken cancellationToken, MultiInstanceFactory factory)
        {
            var eventHandlers = GetHandlers<IEventHandler<TEvent>>(factory)
                .Select(x =>
                {
                    x.Handle(@event);
                    return Unit.Task;
                });

            var asyncEventHandlers = GetHandlers<IAsyncEventHandler<TEvent>>(factory)
                .Select(x => x.HandleAsync(@event));

            var cancellableAsyncEventHandlers = GetHandlers<ICancellableAsyncEventHandler<TEvent>>(factory)
                .Select(x => x.HandleAsync(@event, cancellationToken));

            var allHandlers = eventHandlers.Concat(asyncEventHandlers).Concat(cancellableAsyncEventHandlers);
            return allHandlers;
        }
    }
}
