using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public RunInfo<int> GetRunInfo(IRunInfo runInfo)
        {
            return runInfo as RunInfo<int>;
        }

        public void CheckExistingJob(IRunInfo runInfo, string version)
        {
            using var context = GetDbContextFactory().Create();
            var jobId = GetRunInfo(runInfo).JobId;
            if (!context.Set<TJob>().Any(x => x.Id == jobId && x.Version == version))
                throw new Exception($"The job ({jobId}) either doesn't exist or last ran with an old version of program");
        }

        public IRunInfo CreateJob(IKernel kernel, IRunInfo runInfo, TMetaData metaData)
        {
            using var context = GetDbContextFactory().Create();
            var job = CreateEntityFrameworkJob(kernel, metaData);
            context.Set<TJob>().Add(job);
            context.SaveChanges();
            return new RunInfo<int>(runInfo.Type, job.Id, GetRunInfo(runInfo).RequestId, runInfo.Path.Clone());
        }

        public IRunInfo CreateRequest(IRunInfo runInfo)
        {
            using var context = GetDbContextFactory().Create();
            var request = CreateEntityFrameworkRequest(runInfo);
            var jobId = GetRunInfo(runInfo).JobId;
            context.Set<TRequest>().Add(request);
            context.SaveChanges();
            return new RunInfo<int>(runInfo.Type, jobId, request.Id, runInfo.Path.Clone());
        }

        protected abstract TJob CreateEntityFrameworkJob(IKernel kernel, TMetaData metaData);

        protected abstract TRequest CreateEntityFrameworkRequest(IRunInfo runInfo);

        public IRunInfo GetJobId(IKernel kernel, IRunInfo runInfo, IMetaData metaData)
        {
            if (GetRunInfo(runInfo).JobId == 0)
            {
                return CreateJob(kernel, runInfo, metaData as TMetaData);
            }
            else
            {
                CheckExistingJob(runInfo, kernel.Version);
                return runInfo;
            }
        }

        public IMetaData GetMetaData(IRunInfo runInfo)
        {
            using var context = GetDbContextFactory().Create();
            return context.Set<TJob>().Single(x => x.Id == GetRunInfo(runInfo).JobId).MetaDataJson.FromJson<TMetaData>(); 
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
