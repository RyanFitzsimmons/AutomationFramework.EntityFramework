using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationFramework.EntityFramework.UnitTests.TestSetup
{
    public class TestEntityFrameworkMetaData : IMetaData
    {
        public string Test { get; set; } = "Test Meta String";
    }
}
