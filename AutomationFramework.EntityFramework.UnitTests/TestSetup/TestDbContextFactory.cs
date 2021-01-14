using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestDbContextFactory : EFDbContextFactory
    {
        public override DbContext Create()
        {
            return new TestDbContext("Data Source=AutomationFrameworkUnitTest.db");
        }
    }
}
