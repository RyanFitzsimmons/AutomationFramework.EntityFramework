using AutomationFramework.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkKernel<TMetaData> : KernelBase<TestEntityFrameworkDataLayer<TMetaData>> where TMetaData : class, IMetaData
    {
        public TestEntityFrameworkKernel(int maxParallelChildren, TestEntityFrameworkDataLayer<TMetaData> dataLayer, ILogger logger = null) : base(dataLayer, logger)
        {
            MaxParallelChildren = maxParallelChildren;
        }

        public override string Name => "Entity Framework Unit Test Job";

        public override string Version => "1";

        private int MaxParallelChildren { get; }

        protected override IStageBuilder Configure(IRunInfo runInfo)
        {
            return GetStageBuilder<TestEntityFrameworkModule>(runInfo)
                .Configure((ri, sp, md) => new(ri, sp, md)
                {
                    Name = "Test EntityFramework Stage",
                    IsEnabled = true,
                    MaxParallelChildren = MaxParallelChildren
                });
        }
    }
}
