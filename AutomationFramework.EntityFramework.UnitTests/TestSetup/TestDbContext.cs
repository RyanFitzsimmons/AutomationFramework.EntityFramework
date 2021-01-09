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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntityFrameworkRequest>()
                .HasOne<TestEntityFrameworkJob>()
                .WithMany()
                .HasForeignKey(r => r.JobId);

            modelBuilder.Entity<TestEntityFrameworkStage>()
                .HasOne<TestEntityFrameworkRequest>()
                .WithMany()
                .HasForeignKey(r => r.RequestId);

            modelBuilder.Entity<TestEntityFrameworkStage>()
                .HasOne<TestEntityFrameworkJob>()
                .WithMany()
                .HasForeignKey(r => r.JobId);

            modelBuilder.Entity<TestEntityFrameworkStage>()
                .Property(e => e.Path)
                .HasConversion(
                    v => v.ToString(),
                    v => StagePath.Parse(v));

            modelBuilder.Entity<TestEntityFrameworkRequest>()
                .Property(e => e.Path)
                .HasConversion(
                    v => v.ToString(),
                    v => StagePath.Parse(v));
        }
    }
}
