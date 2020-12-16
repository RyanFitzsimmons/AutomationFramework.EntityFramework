using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutomationFramework.EntityFramework
{
    internal static class ModuleHelper
    {
        internal static TStage GetStage<TStage>(RunInfo<int> runInfo, StagePath path, DbContext context)
            where TStage : Stage
        {
            return context.Set<TStage>().Single(x => x.JobId == runInfo.JobId && x.RequestId == runInfo.RequestId && x.PathString == path.ToString());
        }

        internal static TStage GetLastStageWithResult<TStage>(RunInfo<int> runInfo, StagePath path, DbContext context)
            where TStage : Stage
        {
            return context.Set<TStage>().Where(x => x.JobId == runInfo.JobId && x.PathString == path.ToString() && x.ResultJson != null).OrderBy(x => x.RequestId).LastOrDefault();
        }

        internal static void CreateStage<TDbContext, TStage>(DbContextFactory<TDbContext> dbContextFactory, TStage stage) 
            where TDbContext : DbContext
            where TStage : Stage
        {
            using var context = dbContextFactory.Create();
            context.Set<TStage>().Add(stage);
            context.SaveChanges();
        }

        internal static void SetStatus<TDbContext, TStage>(DbContextFactory<TDbContext> dbContextFactory, RunInfo<int> runInfo, StagePath path, StageStatuses status)
            where TDbContext : DbContext
            where TStage : Stage
        {
            using var context = dbContextFactory.Create();
            var stage = GetStage<TStage>(runInfo, path, context);
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
    }
}
