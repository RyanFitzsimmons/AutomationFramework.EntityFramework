using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationFramework.EntityFramework
{
    public abstract class KernelDataLayer<TDbContext, TJob, TRequest, TMetaData> : IKernelDataLayer
        where TDbContext : DbContext
        where TJob : Job
        where TRequest : Request
        where TMetaData : class, IMetaData
    {
        protected abstract DbContextFactory GetDbContextFactory();

        public RunInfo<int> GetRunInfo(IRunInfo runInfo)
        {
            return runInfo as RunInfo<int>;
        }

        public bool GetIsEmptyId(int Id)
        {
            return Id == 0;
        }

        public void CheckExistingJob(IRunInfo runInfo, string version)
        {
            using var context = GetDbContextFactory().Create();
            var jobId = GetRunInfo(runInfo).JobId;
            if (!context.Set<TJob>().Any(x => x.Id == jobId && x.Version == version))
                throw new Exception($"The job ({jobId}) either doesn't exist or last ran with an old version of program");
        }

        public IRunInfo CreateJob(IKernel kernel, IRunInfo runInfo)
        {
            using var context = GetDbContextFactory().Create();
            var job = CreateEntityFrameworkJob(kernel);
            context.Set<TJob>().Add(job);
            context.SaveChanges();
            return new RunInfo<int>(runInfo.Type, job.Id, GetRunInfo(runInfo).RequestId, runInfo.Path.Clone());
        }

        public IRunInfo CreateRequest(IRunInfo runInfo, IMetaData metaData)
        {
            using var context = GetDbContextFactory().Create();
            var request = CreateEntityFrameworkRequest(runInfo, metaData as TMetaData);
            var jobId = GetRunInfo(runInfo).JobId;
            context.Set<TRequest>().Add(request);
            context.SaveChanges();
            return new RunInfo<int>(runInfo.Type, jobId, request.Id, runInfo.Path.Clone());
        }

        protected abstract TJob CreateEntityFrameworkJob(IKernel kernel);

        protected abstract TRequest CreateEntityFrameworkRequest(IRunInfo runInfo, TMetaData metaData);

        public IRunInfo GetJobId(IKernel kernel, IRunInfo runInfo)
        {
            if (GetRunInfo(runInfo).JobId == 0)
            {
                return CreateJob(kernel, runInfo);
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
            return context.Set<TRequest>().Single(x => x.Id == GetRunInfo(runInfo).RequestId).MetaDataJson.FromJson<TMetaData>(); 
        }
    }
}
