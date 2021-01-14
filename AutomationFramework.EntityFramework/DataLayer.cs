using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutomationFramework.EntityFramework
{
    public abstract class DataLayer<TDbContext, TJob, TRequest, TMetaData, TStage> : IDataLayer
        where TDbContext : DbContext
        where TJob : Job
        where TRequest : Request
        where TMetaData : class, IMetaData
        where TStage : Stage
    {
        protected abstract DbContextFactory GetDbContextFactory();

        protected abstract TJob CreateEntityFrameworkJob(IKernel kernel);

        protected abstract TRequest CreateEntityFrameworkRequest(IRunInfo runInfo, TMetaData metaData);

        protected abstract TStage CreateEntityFrameworkStage(IModule module);

        public RunInfo<int> GetRunInfo(IRunInfo runInfo) => runInfo as RunInfo<int>;

        public bool GetIsNewJob(IRunInfo runInfo) => GetRunInfo(runInfo).JobId == 0;

        public IRunInfo CreateJob(IKernel kernel, IRunInfo runInfo)
        {
            using var context = GetDbContextFactory().Create();
            var job = CreateEntityFrameworkJob(kernel);
            context.Set<TJob>().Add(job);
            context.SaveChanges();
            return new RunInfo<int>(runInfo.Type, job.Id, GetRunInfo(runInfo).RequestId, runInfo.Path);
        }

        public void ValidateExistingJob(IRunInfo runInfo, string version)
        {
            using var context = GetDbContextFactory().Create();
            var jobId = GetRunInfo(runInfo).JobId;
            if (!context.Set<TJob>().Any(x => x.Id == jobId && x.Version == version))
                throw new Exception($"The job ({jobId}) either doesn't exist or last ran with an old version of program");
        }

        public IRunInfo CreateRequest(IRunInfo runInfo, IMetaData metaData)
        {
            using var context = GetDbContextFactory().Create();
            var request = CreateEntityFrameworkRequest(runInfo, metaData as TMetaData);
            var jobId = GetRunInfo(runInfo).JobId;
            context.Set<TRequest>().Add(request);
            context.SaveChanges();
            return new RunInfo<int>(runInfo.Type, jobId, request.Id, runInfo.Path);
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

        private static TStage GetStage(RunInfo<int> runInfo, StagePath path, DbContext context) =>
            context.Set<TStage>().Single(x => x.JobId == runInfo.JobId && x.RequestId == runInfo.RequestId && x.Path == path);

        private static TStage GetLastStageWithResult(RunInfo<int> runInfo, StagePath path, DbContext context) => 
            context.Set<TStage>().Where(x => x.JobId == runInfo.JobId && x.Path == path && x.ResultJson != null).OrderBy(x => x.RequestId).LastOrDefault();
    }
}
