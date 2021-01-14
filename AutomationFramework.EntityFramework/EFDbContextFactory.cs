using Microsoft.EntityFrameworkCore;

namespace AutomationFramework.EntityFramework
{
    public abstract class EFDbContextFactory
    {
        public abstract DbContext Create();
    }
}
