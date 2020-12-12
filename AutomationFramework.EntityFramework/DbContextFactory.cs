using Microsoft.EntityFrameworkCore;
using System;

namespace AutomationFramework.EntityFramework
{
    public abstract class DbContextFactory<TDbContext> where TDbContext : DbContext
    {
        public abstract DbContext Create();
    }
}
