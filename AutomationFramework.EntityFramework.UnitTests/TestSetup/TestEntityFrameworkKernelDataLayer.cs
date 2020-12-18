using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkKernelDataLayer<TMetaData> : KernelDataLayer<
            TestDbContext,
            TestEntityFrameworkJob,
            TestEntityFrameworkRequest,
            TMetaData> where TMetaData : class, IMetaData
    {
        protected override DbContextFactory GetDbContextFactory()
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

        protected override TestEntityFrameworkRequest CreateEntityFrameworkRequest(IRunInfo runInfo, TMetaData metaData)
        {
            return new TestEntityFrameworkRequest
            {
                JobId = GetRunInfo(runInfo).JobId,
                RunType = runInfo.Type,
                Path = runInfo.Path,
                MetaDataJson = metaData.ToJson(),
            };
        }
    }
}
