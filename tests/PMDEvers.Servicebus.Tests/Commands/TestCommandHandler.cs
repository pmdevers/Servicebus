// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

using PMDEvers.Servicebus;

namespace pmdevers.servicebus.test
{
	public class TestCommandHandler : ICommandHandler<TestCommand>, ICommandHandler<TestCommand2>
	{
		public void Handle(TestCommand command)
		{
		}

        public void Handle(TestCommand2 command)
        {
        }
    }
}
