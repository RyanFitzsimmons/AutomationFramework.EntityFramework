using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutomationFramework.EntityFramework
{
    internal static class ModuleHelper
    {
        internal static TStage GetStage<TStage>(RunInfo runInfo, StagePath path, DbContext context)
            where TStage : Stage
        {
            return context.Set<TStage>().Single(x => x.JobId == (int)runInfo.JobId && x.RequestId == (int)runInfo.RequestId && x.PathIndices == path.Indices);
        }

        internal static TStage GetLastStageWithResult<TStage>(RunInfo runInfo, StagePath path, DbContext context)
            where TStage : Stage
        {
            return context.Set<TStage>().Where(x => x.JobId == (int)runInfo.JobId && x.PathIndices == path.Indices && x.ResultJson != null).OrderBy(x => x.RequestId).LastOrDefault();
        }

        internal static void CreateStage<TDbContext, TStage>(DbContextFactory<TDbContext> dbContextFactory, TStage stage) 
            where TDbContext : DbContext
            where TStage : Stage
        {
            using var context = dbContextFactory.Create();
            context.Set<TStage>().Add(stage);
            context.SaveChanges();
        }

        internal static void SetStatus<TDbContext, TStage>(DbContextFactory<TDbContext> dbContextFactory, RunInfo runInfo, StagePath path, StageStatuses status)
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
