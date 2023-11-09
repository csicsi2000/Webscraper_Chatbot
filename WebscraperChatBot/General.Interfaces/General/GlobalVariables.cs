using System.Reflection;

namespace General.Interfaces.General
{
    public static class GlobalVariables
    {
        public static readonly string TestLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
