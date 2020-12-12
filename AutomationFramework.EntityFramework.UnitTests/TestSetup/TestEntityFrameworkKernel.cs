using AutomationFramework.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkKernel : KernelBase<TestEntityFrameworkKernelDataLayer>
    {
        public TestEntityFrameworkKernel(int maxParallelChildren, ILogger logger = null) : base(logger)
        {
            MaxParallelChildren = maxParallelChildren;
        }

        public override string Name => "Entity Framework Unit Test Job";

        public override string Version => "1";

        private int MaxParallelChildren { get; }

        protected override IModule CreateStages()
        {
            return new TestEntityFrameworkModule()
            {
                Name = "Test EntityFramework Stage",
                IsEnabled = true,
                MaxParallelChildren = MaxParallelChildren,
            };
        }
    }
}
