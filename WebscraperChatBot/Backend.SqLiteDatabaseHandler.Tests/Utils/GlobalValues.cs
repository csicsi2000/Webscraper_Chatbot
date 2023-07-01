using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DatabaseHandler.Tests.Utils
{
    public static class GlobalValues
    {
        public static readonly string TestLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string TestDBPath = Path.Combine(TestLocation, "test.sqlite");
    }
}
