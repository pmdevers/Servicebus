using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Pipeline
{
    public interface ICommandPreProcessor<in TCommand>
    {
        Task ProcessAsync(TCommand command);
    }
}
