using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using pmdevers.servicebus.test;

using PMDEvers.Servicebus.Pipeline;

using Xunit;

namespace PMDEvers.Servicebus.Test
{
    public class PreProcessorTests
    {
        [Fact]
        public void RegisterPreProcessorGeneric_Resolves()
        {
            var services = new ServiceCollection();

            services.AddServiceBus()
                    .AddPreProcessor<TestPreProcessor<TestCommand>, TestCommand>();

            var container = services.BuildServiceProvider();

            var processor = container.GetService<ICommandPreProcessor<TestCommand>>();

            Assert.NotNull(processor);
        }

        //[Fact]
        //public void RegisterPreProcessorType_Resolves()
        //{
        //    var services = new ServiceCollection();
        //    var mock = new Mock<ICommandPreProcessor<TestCommand>>();

        //    services.AddServiceBus()
        //            .AddPreProcessor(mock.Object.GetType());

        //    var container = services.BuildServiceProvider();

        //    var processor = container.GetService<ICommandPreProcessor<TestCommand>>();

        //    Assert.NotNull(processor);
        //}

        [Fact]
        public void RegisterPreProcessorOpenGeneric_Executed()
        {
            var services = new ServiceCollection();

            services.AddServiceBus()
                    .AddCommandHandler<TestCommand, TestCommandHandler>()
                    .AddCommandHandler<TestCommand2, TestCommandHandler>()
                    .AddPreProcessor(typeof(TestPreProcessor<>));

            var container = services.BuildServiceProvider();

            var serviceBus = container.GetService<IServiceBus>();

            serviceBus.SendAsync(new TestCommand(), CancellationToken.None);

            Assert.Equal(1, TestPreProcessor<TestCommand>.Hitcount);

            serviceBus.SendAsync(new TestCommand2(), CancellationToken.None);

            Assert.Equal(2, TestPreProcessor<TestCommand>.Hitcount);
        }

        [Fact]
        public void RegisterPreProcessorOpenGeneric_Resolves()
        {
            var services = new ServiceCollection();

            services.AddServiceBus()
                    .AddPreProcessor(typeof(TestPreProcessor<>));

            var container = services.BuildServiceProvider();

            var processor = container.GetService<ICommandPreProcessor<TestCommand>>();
            var processor1 = container.GetService<ICommandPreProcessor<TestCommand2>>();

            Assert.NotNull(processor);
            Assert.NotNull(processor1);


        }
    }

    public class TestPreProcessor<TCommand> : ICommandPreProcessor<TCommand>
    {
        public static int Hitcount = 0;
        public Task ProcessAsync(TCommand command)
        {
            Hitcount++;
            return Task.CompletedTask;
        }
    }
}
