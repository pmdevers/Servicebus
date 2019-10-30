using System;
using System.Collections.Generic;
using System.Text;

using PMDEvers.Servicebus.Interfaces;

namespace PMDEvers.Servicebus.Tests
{
    public class TestQueryHandler : IQueryHandler<TestQuery, TestResult>
    {
        public TestResult Handle(TestQuery query)
        {
            return new TestResult();
        }
    }
}
