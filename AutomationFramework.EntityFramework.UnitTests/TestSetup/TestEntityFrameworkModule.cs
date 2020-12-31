using AutomationFramework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkModule : Module<TestEntityFrameworkModuleDataLayer, TestEntityFrameworkResult>
    {
        public TestEntityFrameworkModule(IRunInfo runInfo, StagePath stagePath, IMetaData metaData) : base(runInfo, stagePath, metaData)
        {
        }

        public override string Name { get; init; } = "Test EntityFramework Stage";

    }
}
