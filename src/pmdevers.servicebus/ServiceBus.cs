﻿// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using PMDEvers.Servicebus.Internal;

using EventHandler = PMDEvers.Servicebus.Internal.EventHandler;

namespace PMDEvers.Servicebus
{
	public class ServiceBus : IServiceBus
	{
		private static readonly ConcurrentDictionary<Type, object> commandHandlers = new ConcurrentDictionary<Type, object>();

		private static readonly ConcurrentDictionary<Type, EventHandler> eventHandlers =
			new ConcurrentDictionary<Type, EventHandler>();

		private static readonly ConcurrentDictionary<Type, CommandHandler> voidCommandHandlers =
			new ConcurrentDictionary<Type, CommandHandler>();

		private readonly MultiInstanceFactory _multiInstanceFactory;
		private readonly SingleInstanceFactory _singleInstanceFactory;

		public ServiceBus(SingleInstanceFactory singleInstanceFactory, MultiInstanceFactory multiInstanceFactory)
		{
			_singleInstanceFactory = singleInstanceFactory ?? throw new ArgumentNullException(nameof(singleInstanceFactory));
			_multiInstanceFactory = multiInstanceFactory ?? throw new ArgumentNullException(nameof(multiInstanceFactory));
		}

		public Task<TResult> SendAsync<TResult>(ICommand<TResult> command,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			Type commandType = command.GetType();
			var handler = (CommandHandler<TResult>)commandHandlers.GetOrAdd(commandType,
				t => Activator.CreateInstance(typeof(CommandHandlerImpl<,>).MakeGenericType(commandType, typeof(TResult))));
			return handler.HandleAsync(command, cancellationToken, _singleInstanceFactory, _multiInstanceFactory);
		}

		public Task SendAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
		{
			Type commandType = command.GetType();
			CommandHandler handler = voidCommandHandlers.GetOrAdd(commandType,
				t => (CommandHandler)Activator.CreateInstance(typeof(CommandHandlerImpl<>).MakeGenericType(commandType)));
			return handler.HandleAsync(command, cancellationToken, _singleInstanceFactory, _multiInstanceFactory);
		}

		public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default(CancellationToken))
			where TEvent : IEvent
		{
			Type notificationType = @event.GetType();
			EventHandler handler = eventHandlers.GetOrAdd(notificationType,
				t => (EventHandler)Activator.CreateInstance(typeof(Internal.EventHandler<>).MakeGenericType(notificationType)));

			return handler.HandleAsync(@event, cancellationToken, _multiInstanceFactory, PublishCore);
		}

		protected virtual Task PublishCore(IEnumerable<Task> allHandlers)
		{
			return Task.WhenAll(allHandlers);
		}
	}
}
