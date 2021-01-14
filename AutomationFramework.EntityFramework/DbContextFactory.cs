using Microsoft.EntityFrameworkCore;

namespace AutomationFramework.EntityFramework
{
    public abstract class DbContextFactory
    {
        public abstract DbContext Create();
    }
}
