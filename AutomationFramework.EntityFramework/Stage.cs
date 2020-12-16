using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

namespace AutomationFramework.EntityFramework
{
    public abstract class Stage
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int RequestId { get; set; }
        [Column("Path")]
        public string PathString { get; set; }
        [NotMapped]
        public StagePath Path { get => StagePath.Parse(PathString); set => PathString = value.ToString(); }
        public string Name { get; set; }
        public StageStatuses Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        [Column("Result")]
        public string ResultJson { get; set; }

        public TResult GetResult<TResult>() where TResult : class => ResultJson?.FromJson<TResult>();

        public void SetResult<TResult>(TResult result) where TResult : class => ResultJson = result?.ToJson();
    }
}
