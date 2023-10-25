using Backend.SqLiteDatabaseHandler.Logic;
using General.Interfaces.Backend;
using General.Interfaces.Data;
using log4net;
using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;

namespace Backend.SqLiteDatabaseHandler
{
    public class SqLiteDataBaseComponent : IDatabaseHandler, IDisposable
    {
        private static readonly ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        DatabaseContext dbContext;
        public SqLiteDataBaseComponent(string connectionString, bool tryCreateFile = false)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            dbContext = new DatabaseContext(connectionString);
            if (tryCreateFile)
            {
                if (!File.Exists(connectionString))
                {
                    SQLiteConnection.CreateFile(connectionString);
                }
            }
        }

        public void DeleteAll()
        {
            dbContext.Contexts.ExecuteDelete();
            dbContext.Files.ExecuteDelete();
        }

        public bool DeleteContext(string text)
        {
            try
            {
                var contexts = dbContext.Contexts.Where(x => x.Text == text);
                if (!contexts.Any())
                {
                    _log4.Info("No context found for deletion");
                    return false;
                }
                dbContext.Contexts.RemoveRange(contexts);
                dbContext.SaveChanges();
                _log4.Info("All context deleted");
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
                _log4.Info("All file deleted");
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
                _log4.Info("All context read");
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
                _log4.Info("Html files red");

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
                _log4.Info("All Html files red");

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
                var contextEntity = Adapters.GetContextEntity(context);
                contextEntity.FileEntity = dbContext.Files.FirstOrDefault(x => x.Url == contextEntity.OriginUrl);
                dbContext.Contexts.Add(contextEntity);
                dbContext.SaveChanges();
                _log4.Info("New context added");

            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }

        public bool InsertOrUpdateHtmlFile(IHtmlFile file)
        {
            try
            {
                var existingFile = dbContext.Files.FirstOrDefault(x => x.Url == file.Url);
                if (existingFile != null)
                {
                    existingFile.Content = file.Content;
                    existingFile.LastModified = file.LastModified;
                    _log4.Info("Existing html modified: " + existingFile.Url);

                }
                else
                {
                    dbContext.Files.Add(Adapters.GetHtmlFileEntity(file));
                    _log4.Info("New html added: " + file.Url);
                }
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
