using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkKernelDataLayer : KernelDataLayer<
            TestDbContext,
            TestEntityFrameworkJob,
            TestEntityFrameworkRequest,
            TestEntityFrameworkMetaData>
    {
        protected override DbContextFactory<TestDbContext> GetDbContextFactory()
        {
            return new TestDbContextFactory();
        }

        protected override TestEntityFrameworkJob CreateEntityFrameworkJob(IKernel kernel)
        {
            return new TestEntityFrameworkJob
            {
                Name = kernel.Name,
                Version = kernel.Version,
                CreatedAt = DateTime.Now,
            };
        }

        protected override TestEntityFrameworkRequest CreateEntityFrameworkRequest(IRunInfo runInfo, TestEntityFrameworkMetaData metaData)
        {
            return new TestEntityFrameworkRequest
            {
                JobId = GetRunInfo(runInfo).JobId,
                RunType = runInfo.Type,
                Path = runInfo.Path,
                MetaData = metaData,
            };
        }
    }
}
