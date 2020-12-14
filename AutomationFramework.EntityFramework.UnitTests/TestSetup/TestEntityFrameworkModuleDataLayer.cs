using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkModuleDataLayer : ModuleDataLayer<TestDbContext, TestEntityFrameworkStage>
    {
        protected override DbContextFactory<TestDbContext> GetDbContextFactory()
        {
            return new TestDbContextFactory();
        }

        protected override TestEntityFrameworkStage CreateEntityFrameworkStage(IModule<int> module)
        {
            return new TestEntityFrameworkStage
            {
                JobId = (int)module.RunInfo.JobId,
                Path = module.StagePath,
                Name = module.Name,
                Status = StageStatuses.None,
            };
        }
    }
}
