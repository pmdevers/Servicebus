using System;
using System.Collections.Generic;
using System.Text;

using AutoFixture;
using AutoFixture.Idioms;

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
    }
}
