// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace PMDEvers.Servicebus
{
	public interface IEventHandler<in TEvent>
		where TEvent : IEvent
	{
		void Handle(TEvent @event);
	}
}
