using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
    public interface IAsyncCommandHandler<in TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        Task<TResponse> HandleAsync(TCommand command);
    }

    public interface IAsyncCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}
