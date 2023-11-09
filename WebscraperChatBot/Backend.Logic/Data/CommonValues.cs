using System.Reflection;

namespace Backend.Logic.Data
{
    internal static class CommonValues
    {
        public static string folderLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
