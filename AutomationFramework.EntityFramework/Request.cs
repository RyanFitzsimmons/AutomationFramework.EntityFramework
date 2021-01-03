﻿using System;
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
        [Column("Path")]
        public string PathString { get; set; }
        [NotMapped]
        public StagePath Path { get => StagePath.Parse(PathString); set => PathString = value.ToString(); }
    }
}
