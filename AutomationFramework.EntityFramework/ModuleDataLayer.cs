using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework
{
    public abstract class ModuleDataLayer<TDbContext, TStage> : IModuleDataLayer
        where TDbContext : DbContext
        where TStage : Stage
    {
        protected abstract DbContextFactory<TDbContext> GetDbContextFactory();

        public RunInfo<int> GetRunInfo(IRunInfo runInfo)
        {
            return runInfo as RunInfo<int>;
        }

        public void CreateStage(IModule module)
        {
            ModuleHelper.CreateStage(GetDbContextFactory(), CreateEntityFrameworkStage(module));
        }

        public TResult GetCurrentResult<TResult>(IModule module) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = ModuleHelper.GetStage<TStage>(GetRunInfo(module.RunInfo), module.StagePath, context);
            return stage?.GetResult<TResult>();
        }

        public TResult GetPreviousResult<TResult>(IModule module) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = ModuleHelper.GetLastStageWithResult<TStage>(GetRunInfo(module.RunInfo), module.StagePath, context);
            return stage?.GetResult<TResult>();
        }

        public void SaveResult<TResult>(IModule module, TResult result) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = ModuleHelper.GetStage<TStage>(GetRunInfo(module.RunInfo), module.StagePath, context);
            stage.SetResult(result);
            context.SaveChanges();
        }

        public void SetStatus(IModule module, StageStatuses status)
        {
            ModuleHelper.SetStatus<TDbContext, TStage>(GetDbContextFactory(), GetRunInfo(module.RunInfo), module.StagePath, status);
        }

        protected abstract TStage CreateEntityFrameworkStage(IModule module);

    }
}
