// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


using Microsoft.Extensions.DependencyInjection;

using PMDEvers.Servicebus;

using Xunit;

namespace pmdevers.servicebus.test
{
	public partial class ServiceBusBuilderTest
	{
		[Fact]
		public void ServiceBus()
		{
			var container = new ServiceCollection();

			container.AddServiceBus();

			ServiceProvider services = container.BuildServiceProvider();

			Assert.Equal(5, container.Count);
			Assert.NotNull(services.GetRequiredService<IServiceBus>());
			Assert.NotNull(services.GetRequiredService<SingleInstanceFactory>());
			Assert.NotNull(services.GetRequiredService<MultiInstanceFactory>());
		}

		[Fact]
		public void ServiceBusBuiler_AddHandler_Test()
		{
			var container = new ServiceCollection();

			container.AddServiceBus()
			         .AddCommandHandler<TestCommand, TestCommandHandler>();

			ServiceProvider service = container.BuildServiceProvider();

			Assert.NotNull(service.GetService<ICommandHandler<TestCommand>>());
		}
	}
}
