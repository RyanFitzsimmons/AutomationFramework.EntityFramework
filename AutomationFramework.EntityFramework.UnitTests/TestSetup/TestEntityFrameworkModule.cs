using AutomationFramework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkModule : Module<TestEntityFrameworkModuleDataLayer, TestEntityFrameworkResult>
    {
        public override string Name { get; set; } = "Test EntityFramework Stage";

    }
}
