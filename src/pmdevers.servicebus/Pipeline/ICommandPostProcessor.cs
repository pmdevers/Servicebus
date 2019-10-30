using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Pipeline
{
    public interface ICommandPostProcessor<in TCommand, in TResponse>
    {
        Task ProcessAsync(TCommand command, TResponse response);
    }
}
