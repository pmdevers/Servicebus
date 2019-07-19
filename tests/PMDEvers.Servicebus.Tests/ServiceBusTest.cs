using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Kernel;

using pmdevers.servicebus.test;

using Xunit;

namespace PMDEvers.Servicebus.Test
{
    public class ServiceBusTest
    {
        [Fact]
        public void ServiceBus_GuardClause()
        {
            var fixture = new Fixture();
            var assertion = new GuardClauseAssertion(fixture);
            assertion.Verify(typeof(ServiceBus).GetConstructors());
        }

        [Fact]
        public async Task ServiceBus_SendAsync()
        {
            var fixture = new Fixture();
            fixture.Inject<SingleInstanceFactory>(new SpecimenContext(fixture).Resolve);
            fixture.Inject<MultiInstanceFactory>(type => new[] { new SpecimenContext(fixture).Resolve(type) });
            fixture.Customize(new AutoMoqCustomization());
            var bus = fixture.Create<ServiceBus>();
            await bus.SendAsync(new TestCommand());
        }
    }
}
