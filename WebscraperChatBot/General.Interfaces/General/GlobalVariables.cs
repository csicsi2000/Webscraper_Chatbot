using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.General
{
    public static class GlobalVariables
    {
        public static readonly string TestLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
