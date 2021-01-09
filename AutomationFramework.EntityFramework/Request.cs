using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

namespace AutomationFramework.EntityFramework
{
    public abstract class Request
    {
        public int Id { get; set; }
        public RunType RunType { get; set; }
        public int JobId { get; set; }
        public StagePath Path { get; set; }
        [Column("MetaData")]
        public string MetaDataJson { get; set; }
    }
}
