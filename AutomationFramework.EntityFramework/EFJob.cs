using System;
using System.ComponentModel.DataAnnotations;

namespace AutomationFramework.EntityFramework
{
    public abstract class EFJob
    {
        [Key]
        public int Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
