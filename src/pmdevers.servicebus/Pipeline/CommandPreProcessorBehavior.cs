// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Pipeline
{
	public class CommandPreProcessorBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
	{
		private readonly IEnumerable<ICommandPreProcessor<TCommand>> _preProcessors;

		public CommandPreProcessorBehavior(IEnumerable<ICommandPreProcessor<TCommand>> preProcessors)
		{
			_preProcessors = preProcessors;
		}

		public async Task<TResponse> HandleAsync(TCommand request, CommandHandlerDelegate<TResponse> next)
		{
			await Task.WhenAll(_preProcessors.Select(p => p.ProcessAsync(request))).ConfigureAwait(false);

			return await next().ConfigureAwait(false);
		}
	}
}
