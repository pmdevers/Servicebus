using System.Threading;
using System.Threading.Tasks;

using PMDEvers.Servicebus.Interfaces;

namespace PMDEvers.Servicebus
{
    public interface IServiceBus
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : class, IQuery<TResult>;

        Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command,
            CancellationToken cancellationToken = default);

        Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
