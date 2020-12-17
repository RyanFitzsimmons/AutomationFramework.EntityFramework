using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework
{
    public abstract class Job<TRequest, TMetaData>
        where TRequest : Request<TMetaData>
        where TMetaData : class, IMetaData
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual List<TRequest> Requests { get; set; } = new List<TRequest>();
    }
}
