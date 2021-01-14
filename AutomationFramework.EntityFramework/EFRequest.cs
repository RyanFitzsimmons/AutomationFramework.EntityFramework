using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomationFramework.EntityFramework
{
    public abstract class EFRequest
    {
        [Key]
        public int Id { get; set; }
        public RunType RunType { get; set; }
        public int JobId { get; set; }
        public StagePath Path { get; set; }
        [Column("MetaData")]
        public string MetaDataJson { get; set; }
    }
}
