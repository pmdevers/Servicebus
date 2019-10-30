using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using PMDEvers.Servicebus.Pipeline;

using pmdevers.servicebus.test;

using PMDEvers.Servicebus.Test;

using Xunit;

namespace PMDEvers.Servicebus.Tests.Pipelines
{
    public class PostProcessorTest
    {
        [Fact]
        public void RegisterPostProcessor()
        {
            var services = new ServiceCollection();

            services.AddServiceBus()
                    .AddCommandHandler<TestCommand, TestCommandHandler>()
                    .AddCommandHandler<TestCommand2, TestCommandHandler>()
                    .AddPostProcessor(typeof(TestPostProcessor<,>));


            var container = services.BuildServiceProvider();
            var serviceBus = container.GetService<IServiceBus>();

            serviceBus.SendAsync(new TestCommand());

        }
    }

    public class TestPostProcessor<TCommand, TResult> : ICommandPostProcessor<TCommand, TResult>
    {
        public Task ProcessAsync(TCommand command, TResult response)
        {
            return Task.CompletedTask;
        }
    }
}
