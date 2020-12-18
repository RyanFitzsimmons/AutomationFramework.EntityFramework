using Microsoft.EntityFrameworkCore;
using System;

namespace AutomationFramework.EntityFramework
{
    public abstract class DbContextFactory
    {
        public abstract DbContext Create();
    }
}
