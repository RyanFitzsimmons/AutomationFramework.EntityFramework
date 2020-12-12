using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(string connectionString) 
            : base(new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connectionString).Options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TestEntityFrameworkJob> Jobs { get; set; }
        public DbSet<TestEntityFrameworkRequest> Requests { get; set; }
        public DbSet<TestEntityFrameworkStage> Stages { get; set; }

    }
}
