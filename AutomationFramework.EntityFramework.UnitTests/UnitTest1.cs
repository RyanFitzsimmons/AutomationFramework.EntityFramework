using AutomationFramework.EntityFramework.UnitTests.TestSetup;
using Serilog;
using System;
using Xunit;
using Xunit.Abstractions;

namespace AutomationFramework.EntityFramework.UnitTests
{
    public class UnitTest1
    {
        public UnitTest1(ITestOutputHelper output)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Xunit(output)
                .CreateLogger();
        }

        [Fact]
        public void Test1()
        {
            var job = new TestEntityFrameworkKernel<MetaData>(1, new TestLogger());
            job.Run(RunInfo<int>.Empty, new MetaData());
            //Test1Results(job);
        }
    }
}
