// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

using PMDEvers.Servicebus.Pipeline;
using PMDEvers.Servicebus;

namespace PMDEvers.Servicebus
{
	public static class ServiceCollectionExtensions
	{
		public static ServiceBusBuilder AddServiceBus(this IServiceCollection services)
		{
			services.AddScoped<IServiceBus, ServiceBus>();

			services.AddScoped<SingleInstanceFactory>(p => p.GetRequiredService);
			services.AddScoped<MultiInstanceFactory>(p => p.GetServices);

			services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandPreProcessorBehavior<,>));
			services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandPostProcessorBehavior<,>));

			return new ServiceBusBuilder(services);
		}
	}
}
