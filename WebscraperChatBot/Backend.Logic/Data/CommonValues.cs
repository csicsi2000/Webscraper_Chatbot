using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logic.Data
{
    internal static class CommonValues
    {
        public static string folderLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
