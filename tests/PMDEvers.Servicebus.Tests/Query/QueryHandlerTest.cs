using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using PMDEvers.Servicebus.Interfaces;
using PMDEvers.Servicebus.Pipeline;

using pmdevers.servicebus.test;

using PMDEvers.Servicebus.Test;

using Xunit;

namespace PMDEvers.Servicebus.Tests
{
    public class QueryHandlerTest
    {
        [Fact]
        public async Task Calls_QueryHandler()
        {
            var services = new ServiceCollection();

            services.AddServiceBus()
                    .AddQueryHandler<TestQuery, TestResult, TestQueryHandler>();

            var container = services.BuildServiceProvider();
            var serviceBus = container.GetService<IServiceBus>();
            var query = new TestQuery();

            var result = await serviceBus.QueryAsync(query);

            Assert.NotNull(result);
        }
    }
}
