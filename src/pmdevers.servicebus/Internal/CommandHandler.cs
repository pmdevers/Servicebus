// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
[assembly:InternalsVisibleTo("PMDEvers.Servicebus.Tests")]

namespace PMDEvers.Servicebus.Internal
{
    internal abstract class CommandHandlerBase
	{
		protected static object GetHandler(Type commandType, SingleInstanceFactory singleInstanceFactory,
			ref Collection<Exception> resolveExceptions)
		{
			try
			{
				return singleInstanceFactory(commandType);
			}
			catch (Exception e)
			{
				resolveExceptions?.Add(e);
				return null;
			}
		}

		protected static THandler GetHandler<THandler>(SingleInstanceFactory factory, ref Collection<Exception> resolveExceptions)
		{
			return (THandler)GetHandler(typeof(THandler), factory, ref resolveExceptions);
		}

		protected static THandler GetHandler<THandler>(SingleInstanceFactory factory)
		{
			Collection<Exception> swallowedExceptions = null;
			return (THandler)GetHandler(typeof(THandler), factory, ref swallowedExceptions);
		}

		protected static InvalidOperationException BuildException(object message, Collection<Exception> resolveExceptions)
		{
			Exception innerException = null;
			if (resolveExceptions.Count == 1)
				innerException = resolveExceptions.First();
			else if (resolveExceptions.Count > 1)
				innerException = new AggregateException("Errors were encountered while resolving handlers", resolveExceptions);

			return new InvalidOperationException(
				"Handler was not found for request of type " + message.GetType() +
				".\r\nContainer or service locator not configured properly or handlers not registered with your container.",
				innerException);
		}
	}

	internal abstract class CommandHandler : CommandHandlerBase
	{
		public abstract Task HandleAsync(ICommand command, CancellationToken cancellationToken,
			SingleInstanceFactory singleFactory, MultiInstanceFactory multiFactory);
	}

	internal abstract class CommandHandler<TResult> : CommandHandlerBase
	{
		public abstract Task<TResult> HandleAsync(ICommand<TResult> command, CancellationToken cancellationToken,
			SingleInstanceFactory singleFactory, MultiInstanceFactory multiFactory);
	}

	internal class CommandHandlerImpl<TCommand, TResult> : CommandHandler<TResult>
		where TCommand : ICommand<TResult>
	{
		private Func<TCommand, CancellationToken, SingleInstanceFactory, CommandHandlerDelegate<TResult>> _handlerFactory;
		private bool _initialized;
		private object _syncLock = new object();

		public override Task<TResult> HandleAsync(ICommand<TResult> command, CancellationToken cancellationToken,
			SingleInstanceFactory singleFactory, MultiInstanceFactory multiFactory)
		{
			var handler = GetHandler((TCommand)command, cancellationToken, singleFactory);

			var pipeline = GetPipeline((TCommand)command, handler, multiFactory);

			return pipeline;
		}

		private CommandHandlerDelegate<TResult> GetHandler(TCommand command, CancellationToken cancellationToken,
			SingleInstanceFactory factory)
		{
			var resolveExceptions = new Collection<Exception>();
			LazyInitializer.EnsureInitialized(ref _handlerFactory, ref _initialized, ref _syncLock,
				() => GetHandlerFactory(factory, ref resolveExceptions));

			if (!_initialized || _handlerFactory == null)
				throw BuildException(command, resolveExceptions);

			return _handlerFactory(command, cancellationToken, factory);
		}

		private static Func<TCommand, CancellationToken, SingleInstanceFactory, CommandHandlerDelegate<TResult>>
			GetHandlerFactory(SingleInstanceFactory factory, ref Collection<Exception> resolveExceptions)
		{
			if (GetHandler<ICommandHandler<TCommand, TResult>>(factory, ref resolveExceptions) != null)
				return (command, token, fac) => () =>
				{
					var handler = GetHandler<ICommandHandler<TCommand, TResult>>(fac);
					return Task.FromResult(handler.Handle(command));
				};

			if (GetHandler<IAsyncCommandHandler<TCommand, TResult>>(factory, ref resolveExceptions) != null)
				return (command, token, fac) =>
				{
					var handler = GetHandler<IAsyncCommandHandler<TCommand, TResult>>(fac);
					return () => handler.HandleAsync(command);
				};

			if (GetHandler<ICancellableAsyncCommandHandler<TCommand, TResult>>(factory, ref resolveExceptions) != null)
				return (command, token, fac) =>
				{
					var handler = GetHandler<ICancellableAsyncCommandHandler<TCommand, TResult>>(fac);
					return () => handler.HandleAsync(command, token);
				};

			return null;
		}

		private static Task<TResult> GetPipeline(TCommand command, CommandHandlerDelegate<TResult> invokeHandler,
			MultiInstanceFactory factory)
		{
			var behaviors = factory(typeof(IPipelineBehavior<TCommand, TResult>))
				.Cast<IPipelineBehavior<TCommand, TResult>>()
				.Reverse();

			var aggregate =
				behaviors.Aggregate(invokeHandler, (next, pipeline) => () => pipeline.HandleAsync(command, next));

			return aggregate();
		}
	}

	internal class CommandHandlerImpl<TCommand> : CommandHandler
		where TCommand : ICommand
	{
		private Func<TCommand, CancellationToken, SingleInstanceFactory, CommandHandlerDelegate<Unit>> _handlerFactory;
		private bool _initialized;
		private object _syncLock = new object();

		public override Task HandleAsync(ICommand command, CancellationToken cancellationToken,
			SingleInstanceFactory singleFactory, MultiInstanceFactory multiFactory)
		{
			var handler = GetHandler((TCommand)command, cancellationToken, singleFactory);

			var pipeline = GetPipeline((TCommand)command, handler, multiFactory);

			return pipeline;
		}

		private CommandHandlerDelegate<Unit> GetHandler(TCommand command, CancellationToken cancellationToken,
			SingleInstanceFactory singleInstanceFactory)
		{
			var resolveExceptions = new Collection<Exception>();
			LazyInitializer.EnsureInitialized(ref _handlerFactory, ref _initialized, ref _syncLock,
				() => GetHandlerFactory(singleInstanceFactory, ref resolveExceptions));

			if (!_initialized || _handlerFactory == null)
				throw BuildException(command, resolveExceptions);

			return _handlerFactory(command, cancellationToken, singleInstanceFactory);
		}

		private static Func<TCommand, CancellationToken, SingleInstanceFactory, CommandHandlerDelegate<Unit>>
			GetHandlerFactory(SingleInstanceFactory factory, ref Collection<Exception> resolveExceptions)
		{
			if (GetHandler<ICommandHandler<TCommand>>(factory, ref resolveExceptions) != null)
				return (command, token, fac) => () =>
				{
					var handler = GetHandler<ICommandHandler<TCommand>>(fac);
					handler.Handle(command);
					return Task.FromResult(Unit.Value);
				};
			if (GetHandler<IAsyncCommandHandler<TCommand>>(factory, ref resolveExceptions) != null)
				return (command, token, fac) => async () =>
				{
					var handler = GetHandler<IAsyncCommandHandler<TCommand>>(fac);
					await handler.HandleAsync(command).ConfigureAwait(false);
					return Unit.Value;
				};
			if (GetHandler<ICancellableAsyncCommandHandler<TCommand>>(factory, ref resolveExceptions) != null)
				return (command, token, fac) => async () =>
				{
					var handler = GetHandler<ICancellableAsyncCommandHandler<TCommand>>(fac);
					await handler.HandleAsync(command, token).ConfigureAwait(false);
					return Unit.Value;
				};
			return null;
		}

		private static Task<Unit> GetPipeline(TCommand command, CommandHandlerDelegate<Unit> invokeHandler,
			MultiInstanceFactory factory)
		{
			var behaviors = factory(typeof(IPipelineBehavior<TCommand, Unit>))
				.Cast<IPipelineBehavior<TCommand, Unit>>()
				.Reverse();

			var aggregate =
				behaviors.Aggregate(invokeHandler, (next, pipeline) => () => pipeline.HandleAsync(command, next));

			return aggregate();
		}
	}
}
