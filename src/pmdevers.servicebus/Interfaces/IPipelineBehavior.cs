using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
    public delegate Task<TResponse> CommandHandlerDelegate<TResponse>();

    public interface IPipelineBehavior<in TCommand, TResponse>
    {
        Task<TResponse> HandleAsync(TCommand command, CommandHandlerDelegate<TResponse> next);
    }
}
