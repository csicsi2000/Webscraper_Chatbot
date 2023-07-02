using Backend.DatabaseHandler;
using Backend.DatabaseHandler.Tests.Utils;
using System.Data.SQLite;

namespace Backend.SqLiteDatabaseHandler.Tests.Utils
{
    [TestClass]
    public class Setup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            if (File.Exists(GlobalValues.TestDBPath))
            {
                File.Delete(GlobalValues.TestDBPath);
            }
            SQLiteConnection.CreateFile(GlobalValues.TestDBPath);
            GlobalValues.TestDatabase = new DataBaseComponent(GlobalValues.TestDBPath);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            GlobalValues.TestDatabase.Dispose();
        }
    }
}
