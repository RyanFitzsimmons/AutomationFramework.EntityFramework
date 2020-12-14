using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationFramework.EntityFramework
{
    public abstract class KernelDataLayer<TDbContext, TJob, TRequest, TMetaData> : IKernelDataLayer<int>
        where TDbContext : DbContext
        where TJob : Job<TRequest, TMetaData>
        where TRequest : Request<TMetaData>
        where TMetaData : class
    {
        protected abstract DbContextFactory<TDbContext> GetDbContextFactory();

        public bool GetIsEmptyId(int Id)
        {
            return Id == 0;
        }

        public void CheckExistingJob(int id, string version)
        {
            using var context = GetDbContextFactory().Create();
            if (!context.Set<TJob>().Any(x => x.Id == id && x.Version == version))
                throw new Exception($"The job ({id}) either doesn't exist or last ran with an old version of program");
        }

        public int CreateJob(IKernel<int> kernel)
        {
            using var context = GetDbContextFactory().Create();
            var job = CreateEntityFrameworkJob(kernel);
            context.Set<TJob>().Add(job);
            context.SaveChanges();
            return job.Id;
        }

        public int CreateRequest(RunInfo<int> runInfo, object metaData)
        {
            using var context = GetDbContextFactory().Create();
            var request = CreateEntityFrameworkRequest(runInfo, metaData as TMetaData);
            var job = context.Set<TJob>().Single(x => x.Id == runInfo.JobId);
            job.Requests.Add(request);
            context.SaveChanges();
            return request.Id;
        }

        protected abstract TJob CreateEntityFrameworkJob(IKernel<int> kernel);

        protected abstract TRequest CreateEntityFrameworkRequest(RunInfo<int> runInfo, TMetaData metaData);
    }
}
