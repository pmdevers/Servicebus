// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
	public interface IServiceBus
	{
		Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command,
			CancellationToken cancellationToken = default(CancellationToken));

		Task SendAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken));

		Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default(CancellationToken))
			where TEvent : IEvent;
	}
}
