using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AutomationFramework.EntityFramework
{
    public abstract class Job
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        [Column("MetaData")]
        public string MetaDataJson { get; set; }
    }
}
