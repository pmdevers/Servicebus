using System;
using System.Threading;
using System.Threading.Tasks;
using PMDEvers.Servicebus.Interfaces;

namespace PMDEvers.Servicebus
{
    public static class ServiceBusExtensions
    {
        private static readonly TaskFactory Factory = new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        private static void RunSync(Func<Task> func)
        {
            Factory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        private static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return Factory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static void Send(this IServiceBus serviceBus, ICommand command) =>
            RunSync(() => serviceBus.SendAsync(command));

        public static T Send<T>(this IServiceBus serviceBus, ICommand<T> command) =>
            RunSync<T>(() => serviceBus.SendAsync<T>(command));

        public static void Publish(this IServiceBus serviceBus, IEvent @event) =>
            RunSync(() => serviceBus.PublishAsync(@event));
        
        public static T Query<T>(this IServiceBus serviceBus, IQuery<T> query) =>
            RunSync<T>(() => serviceBus.QueryAsync<T>(query));

    }

}
