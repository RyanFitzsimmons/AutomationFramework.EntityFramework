using AutomationFramework.EntityFramework.UnitTests.TestSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework.UnitTests
{
    public class MetaData : IMetaData
    {
        public string Test { get; set; } = "Test Meta String";
    }
}
