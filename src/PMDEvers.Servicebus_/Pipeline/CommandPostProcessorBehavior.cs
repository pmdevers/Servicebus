using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Pipeline
{
    public class CommandPostProcessorBehavior<TCommand, TRepsonse> : IPipelineBehavior<TCommand, TRepsonse>
    {
        private readonly IEnumerable<ICommandPostProcessor<TCommand, TRepsonse>> _postProcessors;

        public CommandPostProcessorBehavior(IEnumerable<ICommandPostProcessor<TCommand, TRepsonse>> postProcessors)
        {
            _postProcessors = postProcessors;
        }

        public async Task<TRepsonse> HandleAsync(TCommand command, CommandHandlerDelegate<TRepsonse> next)
        {
            TRepsonse response = await next().ConfigureAwait(false);
            await Task.WhenAll(_postProcessors.Select(p => p.ProcessAsync(command, response))).ConfigureAwait(false);
            return response;
        }
    }
}
