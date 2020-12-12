using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationFramework.EntityFramework
{
    public abstract class KernelDataLayer<TDbContext, TJob, TRequest, TMetaData> : IKernelDataLayer
        where TDbContext : DbContext
        where TJob : Job<TRequest, TMetaData>
        where TRequest : Request<TMetaData>
        where TMetaData : class
    {
        protected abstract DbContextFactory<TDbContext> GetDbContextFactory();

        public bool GetIsEmptyId(object Id)
        {
            return Id == null || (int)0 == (int)Id;
        }

        public void CheckExistingJob(object id, string version)
        {
            using var context = GetDbContextFactory().Create();
            if (!context.Set<TJob>().Any(x => x.Id == (int)id && x.Version == version))
                throw new Exception($"The job ({(int)id}) either doesn't exist or last ran with an old version of program");
        }

        public object CreateJob(IKernel kernel)
        {
            using var context = GetDbContextFactory().Create();
            var job = CreateEntityFrameworkJob(kernel);
            context.Set<TJob>().Add(job);
            context.SaveChanges();
            return job.Id;
        }

        public object CreateRequest(RunInfo runInfo, object metaData)
        {
            using var context = GetDbContextFactory().Create();
            var request = CreateEntityFrameworkRequest(runInfo, metaData as TMetaData);
            var job = context.Set<TJob>().Single(x => x.Id == (int)runInfo.JobId);
            job.Requests.Add(request);
            context.SaveChanges();
            return request.Id;
        }

        protected abstract TJob CreateEntityFrameworkJob(IKernel kernel);

        protected abstract TRequest CreateEntityFrameworkRequest(RunInfo runInfo, TMetaData metaData);
    }
}
