// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace PMDEvers.Servicebus.Pipeline
{
	public interface ICommandPostProcessor<in TCommand, in TResponse>
	{
		Task ProcessAsync(TCommand command, TResponse response);
	}
}
