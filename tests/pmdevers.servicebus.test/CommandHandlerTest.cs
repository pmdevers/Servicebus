using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Kernel;

using pmdevers.servicebus.test;

using PMDEvers.Servicebus.Internal;

using Xunit;

namespace PMDEvers.Servicebus.Test
{
    public class CommandHandlerTest
    {
		[Fact]
	    public void CommandHandler_Guard_Test()
	    {
			var fixture = new Fixture();
		    var assertion = new GuardClauseAssertion(fixture);
		    assertion.Verify(typeof(CommandHandlerBase).GetConstructors());
		    assertion.Verify(typeof(CommandHandler).GetConstructors());
		    assertion.Verify(typeof(CommandHandlerImpl<>).GetConstructors());
		}

	    public void CommandHandler_HandleAsync_Calls_SingleFactory()
	    {
		    var fixture = new Fixture();
		    fixture.Customize(new AutoMoqCustomization());

		    var handler = fixture.Create<CommandHandlerImpl<TestCommand>>();

			handler.HandleAsync(new TestCommand(), CancellationToken.None, new SpecimenContext(fixture).Resolve, null);
			 
	    }
    }
}
