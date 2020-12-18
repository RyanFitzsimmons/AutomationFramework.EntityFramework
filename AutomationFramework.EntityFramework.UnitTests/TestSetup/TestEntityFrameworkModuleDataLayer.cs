using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkModuleDataLayer : ModuleDataLayer<TestDbContext, TestEntityFrameworkStage>
    {
        protected override DbContextFactory GetDbContextFactory()
        {
            return new TestDbContextFactory();
        }

        protected override TestEntityFrameworkStage CreateEntityFrameworkStage(IModule module)
        {
            var runInfo = GetRunInfo(module.RunInfo);
            return new TestEntityFrameworkStage
            {
                JobId = runInfo.JobId,
                RequestId = runInfo.RequestId,
                Path = module.StagePath,
                Name = module.Name,
                Status = StageStatuses.None,
            };
        }
    }
}
