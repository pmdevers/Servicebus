using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using PMDEvers.Servicebus.Interfaces;

[assembly: InternalsVisibleTo("PMDEvers.Servicebus.Tests")]

namespace PMDEvers.Servicebus.Internal
{
    internal abstract class QueryHandlerBase
    {
        protected static object GetHandler(Type queryType, SingleInstanceFactory singleInstanceFactory,
            ref Collection<Exception> resolveExceptions)
        {
            try
            {
                return singleInstanceFactory(queryType);
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

    internal abstract class QueryHandler<TResult> : QueryHandlerBase
    {
        public abstract Task<TResult> HandleAsync(IQuery<TResult> query, CancellationToken cancellationToken,
            SingleInstanceFactory singleFactory);
    }

    internal class QueryHandlerImpl<TQuery, TResult> : QueryHandler<TResult>
        where TQuery : class, IQuery<TResult>
    {
        private Func<TQuery, CancellationToken, SingleInstanceFactory, CommandHandlerDelegate<TResult>> _handlerFactory;
        private bool _initialized;
        private object _syncLock = new object();

        public override Task<TResult> HandleAsync(IQuery<TResult> query, CancellationToken cancellationToken,
            SingleInstanceFactory singleFactory)
        {
            var handler = GetHandler((TQuery)query, cancellationToken, singleFactory);
            return handler.Invoke();
        }

        private CommandHandlerDelegate<TResult> GetHandler(TQuery query, CancellationToken cancellationToken,
            SingleInstanceFactory factory)
        {
            var resolveExceptions = new Collection<Exception>();
            LazyInitializer.EnsureInitialized(ref _handlerFactory, ref _initialized, ref _syncLock,
                () => GetHandlerFactory(factory, ref resolveExceptions));

            if (!_initialized || _handlerFactory == null)
                throw BuildException(query, resolveExceptions);

            return _handlerFactory(query, cancellationToken, factory);
        }

        private Func<TQuery, CancellationToken, SingleInstanceFactory, CommandHandlerDelegate<TResult>>
            GetHandlerFactory(SingleInstanceFactory factory, ref Collection<Exception> resolveExceptions)
        {
            if (GetHandler<IQueryHandler<TQuery, TResult>>(factory, ref resolveExceptions) != null)
                return (query, token, fac) => () =>
                {
                    var handler = GetHandler<IQueryHandler<TQuery, TResult>>(fac);
                    return Task.FromResult(handler.Handle(query));
                };

            if (GetHandler<IAsyncQueryHandler<TQuery, TResult>>(factory, ref resolveExceptions) != null)
                return (query, token, fac) => () =>
                {
                    var handler = GetHandler<IAsyncQueryHandler<TQuery, TResult>>(fac);
                    return handler.HandleAsync(query);
                };

            if (GetHandler<ICancellableAsyncQueryHandler<TQuery, TResult>>(factory, ref resolveExceptions) != null)
                return (query, token, fac) => () =>
                {
                    var handler = GetHandler<ICancellableAsyncQueryHandler<TQuery, TResult>>(fac);
                    return handler.HandleAsync(query, token);
                };

            return null;
        }
    }
}
