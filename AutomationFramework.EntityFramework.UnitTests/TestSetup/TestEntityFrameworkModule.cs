using AutomationFramework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkModule : Module<TestEntityFrameworkResult>
    {
        public TestEntityFrameworkModule(IDataLayer dataLayer, IRunInfo runInfo, StagePath stagePath) : base(dataLayer, runInfo, stagePath)
        {
        }

        public override string Name { get; init; } = "Test EntityFramework Stage";

    }
}
