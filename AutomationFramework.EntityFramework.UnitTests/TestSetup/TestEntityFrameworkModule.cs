using AutomationFramework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkModule : Module<TestEntityFrameworkResult>
    {
        public TestEntityFrameworkModule(IStageBuilder builder) : base(builder)
        {
        }

        public override string Name { get; init; } = "Test EntityFramework Stage";

    }
}
