// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

using Microsoft.Extensions.DependencyInjection;

using PMDEvers.Servicebus.Pipeline;

namespace PMDEvers.Servicebus
{
	public class ServiceBusBuilder
	{
		private readonly IServiceCollection _services;

		public ServiceBusBuilder(IServiceCollection services)
		{
			_services = services;
		}

		private void AddScoped(Type serviceType, Type concreteType)
		{
			_services.AddScoped(concreteType, serviceType);
		}

		public ServiceBusBuilder AddCommandHandler<TCommand, TCommandHandler>()
			where TCommand : ICommand
			where TCommandHandler : class
		{
			if (typeof(ICommandHandler<TCommand>).IsAssignableFrom(typeof(TCommandHandler)))
				_services.AddScoped(typeof(ICommandHandler<TCommand>), typeof(TCommandHandler));

			if (typeof(IAsyncCommandHandler<TCommand>).IsAssignableFrom(typeof(TCommandHandler)))
				_services.AddScoped(typeof(IAsyncCommandHandler<TCommand>), typeof(TCommandHandler));

			if (typeof(ICancellableAsyncCommandHandler<TCommand>).IsAssignableFrom(typeof(TCommandHandler)))
				_services.AddScoped(typeof(ICancellableAsyncCommandHandler<TCommand>), typeof(TCommandHandler));

			return this;
		}

		public ServiceBusBuilder AddEventHandler<TEvent, TEventHandler>()
			where TEvent : IEvent
			where TEventHandler : class
		{
			if (typeof(IEventHandler<TEvent>).IsAssignableFrom(typeof(TEventHandler)))
				_services.AddScoped(typeof(IEventHandler<TEvent>), typeof(TEventHandler));

			if (typeof(IAsyncEventHandler<TEvent>).IsAssignableFrom(typeof(TEventHandler)))
				_services.AddScoped(typeof(IAsyncEventHandler<TEvent>), typeof(TEventHandler));

			if (typeof(ICancellableAsyncEventHandler<TEvent>).IsAssignableFrom(typeof(TEventHandler)))
				_services.AddScoped(typeof(ICancellableAsyncEventHandler<TEvent>), typeof(TEventHandler));

			return this;
		}

		public ServiceBusBuilder AddPreProcessor<TProcessor, TCommand>()
			where TProcessor : ICommandPreProcessor<TCommand>
			where TCommand : ICommand
		{
			AddScoped(typeof(TProcessor), typeof(ICommandPreProcessor<TCommand>));
			return this;
		}

		public ServiceBusBuilder AddPreProcessor(Type preProcessorType)
		{
		    var interfaceType = typeof(ICommandPreProcessor<>);

            if(preProcessorType.GenericTypeArguments.Length > 0)
		        interfaceType = typeof(ICommandPreProcessor<>).MakeGenericType(preProcessorType.GenericTypeArguments);

			if (preProcessorType.GenericTypeArguments.Length > 0 && !interfaceType.IsAssignableFrom(preProcessorType))
				throw new InvalidOperationException($"Type {preProcessorType.Name} is not a valid PreProcesor");

			AddScoped(preProcessorType, interfaceType);
			return this;
		}

		public ServiceBusBuilder AddPostProcessor<TProcessor, TCommand, TReponse>()
			where TProcessor : ICommandPostProcessor<TCommand, TReponse>
			where TCommand : ICommand
		{
			AddScoped(typeof(TProcessor), typeof(ICommandPostProcessor<TCommand, TReponse>));
			return this;
		}
	}
}
