using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework
{
    public abstract class EFDataLayer<TDbContext, TJob, TRequest, TMetaData, TStage> : IDataLayer
        where TDbContext : DbContext
        where TJob : EFJob
        where TRequest : EFRequest
        where TMetaData : class, IMetaData
        where TStage : EFStage
    {
        protected abstract EFDbContextFactory GetDbContextFactory();

        protected abstract TJob CreateEntityFrameworkJob(IKernel kernel);

        protected abstract TRequest CreateEntityFrameworkRequest(IRunInfo runInfo, TMetaData metaData);

        protected abstract TStage CreateEntityFrameworkStage(IModule module);

        public RunInfo<int> GetRunInfo(IRunInfo runInfo) => runInfo as RunInfo<int>;

        public bool GetIsNewJob(IRunInfo runInfo) => GetRunInfo(runInfo).JobId == 0;

        public async Task<IRunInfo> CreateJob(IKernel kernel, IRunInfo runInfo, CancellationToken token)
        {
            using var context = GetDbContextFactory().Create();
            var job = CreateEntityFrameworkJob(kernel);
            context.Set<TJob>().Add(job);
            await context.SaveChangesAsync(token);
            return new RunInfo<int>(runInfo.Type, job.Id, GetRunInfo(runInfo).RequestId, runInfo.Path);
        }

        public async Task ValidateExistingJob(IRunInfo runInfo, string version, CancellationToken token)
        {
            using var context = GetDbContextFactory().Create();
            var jobId = GetRunInfo(runInfo).JobId;
            if (!await context.Set<TJob>().AnyAsync(x => x.Id == jobId && x.Version == version, token))
                throw new Exception($"The job ({jobId}) either doesn't exist or last ran with an old version of program");
        }

        public async Task<IRunInfo> CreateRequest(IRunInfo runInfo, IMetaData metaData, CancellationToken token)
        {
            using var context = GetDbContextFactory().Create();
            var request = CreateEntityFrameworkRequest(runInfo, metaData as TMetaData);
            var jobId = GetRunInfo(runInfo).JobId;
            context.Set<TRequest>().Add(request);
            await context.SaveChangesAsync(token);
            return new RunInfo<int>(runInfo.Type, jobId, request.Id, runInfo.Path);
        }

        public async Task CreateStage(IModule module, CancellationToken token)
        {
            using var context = GetDbContextFactory().Create();
            context.Set<TStage>().Add(CreateEntityFrameworkStage(module));
            await context.SaveChangesAsync(token);
        }

        public async Task<TResult> GetCurrentResult<TResult>(IModule module, CancellationToken token) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = await GetStage(GetRunInfo(module.RunInfo), module.StagePath, context, token);
            return stage?.GetResult<TResult>();
        }

        public async Task<TResult> GetPreviousResult<TResult>(IModule module, CancellationToken token) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = await GetLastStageWithResult(GetRunInfo(module.RunInfo), module.StagePath, context, token);
            return stage?.GetResult<TResult>();
        }

        public async Task SaveResult<TResult>(IModule module, TResult result, CancellationToken token) where TResult : class
        {
            using var context = GetDbContextFactory().Create();
            var stage = await GetStage(GetRunInfo(module.RunInfo), module.StagePath, context, token);
            stage?.SetResult(result);
            await context.SaveChangesAsync(token);
        }

        public async Task SetStatus(IModule module, StageStatuses status, CancellationToken token)
        {
            using var context = GetDbContextFactory().Create();
            var stage = await GetStage(GetRunInfo(module.RunInfo), module.StagePath, context, token);
            if (token.IsCancellationRequested) return;
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
            await context.SaveChangesAsync(token);
        }

        private static async Task<TStage> GetStage(RunInfo<int> runInfo, StagePath path, DbContext context, CancellationToken token) =>
            await context.Set<TStage>().SingleAsync(x => x.JobId == runInfo.JobId && x.RequestId == runInfo.RequestId && x.Path == path, token);

        private static async Task<TStage> GetLastStageWithResult(RunInfo<int> runInfo, StagePath path, DbContext context, CancellationToken token) => 
            await context.Set<TStage>().Where(x => x.JobId == runInfo.JobId && x.Path == path && x.ResultJson != null).OrderBy(x => x.RequestId).LastOrDefaultAsync(token);
    }
}
