// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
	public interface IAsyncEventHandler<in TEvent>
		where TEvent : IEvent
	{
		Task HandleAsync(TEvent @event);
	}
}
