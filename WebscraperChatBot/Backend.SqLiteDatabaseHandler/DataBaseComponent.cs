using Backend.DatabaseHandler.Data;
using Backend.DatabaseHandler.Logic;
using Backend.SqLiteDatabaseHandler.Logic;
using General.Interfaces.Backend;
using General.Interfaces.Data;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace Backend.DatabaseHandler
{
    public class DataBaseComponent : IDatabaseHandler, IDisposable
    {
        private static readonly ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        DatabaseContext dbContext;
        public DataBaseComponent(string connectionString) 
        {
            if(String.IsNullOrEmpty(connectionString)) 
                throw new ArgumentNullException(nameof(connectionString));

            dbContext = new DatabaseContext(connectionString);
        }

        public DataBaseComponent(DbContextOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            dbContext = new DatabaseContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        public bool DeleteContext(string text)
        {
            try
            {
                var contexts = dbContext.Contexts.Where(x => x.Text == text);
                if(!contexts.Any())
                {
                    _log4.Info("No context found for deletion");
                    return false;
                }
                dbContext.Contexts.RemoveRange(contexts);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }

        public bool DeleteHtmlFile(string url)
        {
            try
            {
                var files = dbContext.Files.Where(x => x.Url == url); 
                if (!files.Any())
                {
                    _log4.Info("No files found for deletion");
                    return false;
                }
                dbContext.Files.RemoveRange(files);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public IEnumerable<IContext> GetContexts()
        {
            try
            {
                var contexts = dbContext.Contexts.ToList();
                return contexts;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return Enumerable.Empty<IContext>();
            }
        }

        public IHtmlFile GetHtmlFile(string url)
        {
            try
            {
                var file = dbContext.Files.FirstOrDefault(x => x.Url == url);
                return file;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return null;
            }
        }

        public IEnumerable<IHtmlFile> GetHtmlFiles()
        {
            try
            {
                var files = dbContext.Files.ToList();
                return files;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return Enumerable.Empty<IHtmlFile>();
            }
        }

        public bool InsertContext(IContext context)
        {
            try
            {
                dbContext.Contexts.Add(Adapters.GetContextEntity(context));
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }

        public bool InsertHtmlFile(IHtmlFile file)
        {
            try
            {
                dbContext.Files.Add(Adapters.GetHtmlFileEntity( file));
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }
    }
}
