using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework
{
    public abstract class ModuleDataLayer<TDbContext, TStage> : IModuleDataLayer<int>
        where TDbContext : DbContext
        where TStage : Stage
    {
        protected abstract DbContextFactory<TDbContext> GetDbContextFactory();

        public void CreateStage(IModule<int> module)
        {
            ModuleHelper.CreateStage(GetDbContextFactory(), CreateEntityFrameworkStage(module));
        }

        public TResult GetCurrentResult<TResult>(IModule<int> module) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = ModuleHelper.GetStage<TStage>(module.RunInfo, module.StagePath, context);
            return stage?.GetResult<TResult>();
        }

        public TResult GetPreviousResult<TResult>(IModule<int> module) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = ModuleHelper.GetLastStageWithResult<TStage>(module.RunInfo, module.StagePath, context);
            return stage?.GetResult<TResult>();
        }

        public void SaveResult<TResult>(IModule<int> module, TResult result) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = ModuleHelper.GetStage<TStage>(module.RunInfo, module.StagePath, context);
            stage.SetResult(result);
            context.SaveChanges();
        }

        public void SetStatus(IModule<int> module, StageStatuses status)
        {
            ModuleHelper.SetStatus<TDbContext, TStage>(GetDbContextFactory(), module.RunInfo, module.StagePath, status);
        }

        protected abstract TStage CreateEntityFrameworkStage(IModule<int> module);

    }
}
