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

        protected override IStageBuilder Configure()
        {
            return GetStageBuilder<TestEntityFrameworkModule>()
                .Configure((m) =>
                {
                    m.Name = "Test EntityFramework Stage";
                    m.IsEnabled = true;
                    m.MaxParallelChildren = MaxParallelChildren;
                });
        }

        protected override TestEntityFrameworkKernelDataLayer CreateDataLayer()
        {
            return new TestEntityFrameworkKernelDataLayer();
        }
    }
}
