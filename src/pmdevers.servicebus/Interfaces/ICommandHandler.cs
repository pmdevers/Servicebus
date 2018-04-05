// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace PMDEvers.Servicebus
{
	public interface ICommandHandler<in TCommand, out TResponse>
		where TCommand : ICommand<TResponse>
	{
		TResponse Handle(TCommand command);
	}

	public interface ICommandHandler<in TCommand>
		where TCommand : ICommand
	{
		void Handle(TCommand command);
	}
}
