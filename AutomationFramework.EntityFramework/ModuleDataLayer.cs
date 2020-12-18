using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework
{
    public abstract class ModuleDataLayer<TDbContext, TStage> : IModuleDataLayer
        where TStage : Stage
    {
        protected abstract DbContextFactory GetDbContextFactory();

        public RunInfo<int> GetRunInfo(IRunInfo runInfo)
        {
            return runInfo as RunInfo<int>;
        }

        public void CreateStage(IModule module)
        {
            using var context = GetDbContextFactory().Create();
            context.Set<TStage>().Add(CreateEntityFrameworkStage(module));
            context.SaveChanges();
        }

        public TResult GetCurrentResult<TResult>(IModule module) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = GetStage(GetRunInfo(module.RunInfo), module.StagePath, context);
            return stage?.GetResult<TResult>();
        }

        public TResult GetPreviousResult<TResult>(IModule module) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = GetLastStageWithResult(GetRunInfo(module.RunInfo), module.StagePath, context);
            return stage?.GetResult<TResult>();
        }

        public void SaveResult<TResult>(IModule module, TResult result) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = GetStage(GetRunInfo(module.RunInfo), module.StagePath, context);
            stage.SetResult(result);
            context.SaveChanges();
        }

        public void SetStatus(IModule module, StageStatuses status)
        {
            using var context = GetDbContextFactory().Create();
            var stage = GetStage(GetRunInfo(module.RunInfo), module.StagePath, context);
            stage.Status = status;
            switch (status)
            {
                case StageStatuses.Bypassed:
                case StageStatuses.Cancelled:
                case StageStatuses.Completed:
                case StageStatuses.Disabled:
                case StageStatuses.Errored:
                    stage.EndedAt = DateTime.Now;
                    break;
                default:
                    break;
            }
            context.SaveChanges();
        }

        protected abstract TStage CreateEntityFrameworkStage(IModule module);

        private static TStage GetStage(RunInfo<int> runInfo, StagePath path, DbContext context)
        {
            return context.Set<TStage>().Single(x => x.JobId == runInfo.JobId && x.RequestId == runInfo.RequestId && x.PathString == path.ToString());
        }

        private static TStage GetLastStageWithResult(RunInfo<int> runInfo, StagePath path, DbContext context)
        {
            return context.Set<TStage>().Where(x => x.JobId == runInfo.JobId && x.PathString == path.ToString() && x.ResultJson != null).OrderBy(x => x.RequestId).LastOrDefault();
        }
    }
}
