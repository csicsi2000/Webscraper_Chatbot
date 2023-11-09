using Backend.SqLiteDatabaseHandler.Data;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.SqLiteDatabaseHandler.Logic
{
    public class DatabaseContext : DbContext
    {
        private static readonly ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DbSet<ContextEntity> Contexts { get; set; }
        public DbSet<HtmlFileEntity> Files { get; set; }

        string _dbPath;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContextEntity>()
                .Property(e => e.Tokens)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }

        public DatabaseContext(string dbPath)
        {
            _dbPath = dbPath ?? throw new ArgumentNullException(nameof(dbPath));
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                databaseCreator.EnsureCreated();
                databaseCreator.CreateTables();
                _log4.Info("Tables are created.");
            }
            catch (Exception ex)
            {
                _log4.Info("Tables exisits.");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={_dbPath}");

        }
    }
}
