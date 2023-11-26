using System.Reflection;

namespace Backend.SqLiteDatabaseHandler.Tests.Utils
{
    public static class GlobalValues
    {
        public static readonly string TestLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string TestDBPath = Path.Combine(TestLocation, "test.sqlite");
        public static SqLiteDataBaseComponent TestDatabase = null;
    }
}
