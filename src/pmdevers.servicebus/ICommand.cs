// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace PMDEvers.Servicebus
{
	public interface ICommand : ICommand<Unit>
	{
	}

	public interface ICommand<out TResponse>
	{
	}
}
