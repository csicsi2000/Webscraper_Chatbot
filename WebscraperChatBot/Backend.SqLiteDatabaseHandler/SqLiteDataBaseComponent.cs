using Backend.SqLiteDatabaseHandler.Logic;
using General.Interfaces.Backend.Components;
using General.Interfaces.Data;
using log4net;
using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;

namespace Backend.SqLiteDatabaseHandler
{
    public class SqLiteDataBaseComponent : IDatabaseHandler, IDisposable
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        DatabaseContext dbContext;
        string _connectionString;
        public SqLiteDataBaseComponent(string connectionString, bool tryCreateFile = false)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
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
        public void Dispose()
        {
            dbContext.Dispose();
        }

        #region Context
        public IQueryable<IContext> GetContexts()
        {
            try
            {
                _log4.Debug("All context read");
                return dbContext.Contexts;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return null;
            }
        }
        public bool DeleteContext(string text)
        {
            try
            {
                var contexts = dbContext.Contexts.Where(x => x.Text == text);
                if (!contexts.Any())
                {
                    _log4.Debug("No context found for deletion");
                    return false;
                }
                dbContext.Contexts.RemoveRange(contexts);
                dbContext.SaveChanges();
                _log4.Debug("All context deleted");
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }
        public bool InsertOrUpdateContext(IContext context)
        {
            try
            {
                using (var tempContext = new DatabaseContext(_connectionString))
                {
                    var samePage = tempContext.Contexts.FirstOrDefault(x => x.OriginUrl == context.OriginUrl);
                    if (samePage != null)
                    {
                        samePage.Text = context.Text;
                        samePage.Tokens = context.Tokens;
                        _log4.Debug("Existing context updated");
                    }
                    else
                    {
                        var contextEntity = Adapters.GetContextEntity(context);
                        contextEntity.FileEntity = tempContext.Files.FirstOrDefault(x => x.Url == contextEntity.OriginUrl);
                        tempContext.Contexts.Add(contextEntity);
                        _log4.Debug("New context added");
                    }
                    tempContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }

        #endregion

        #region HtmlFiles
        public bool DeleteHtmlFile(string url)
        {
            try
            {
                var files = dbContext.Files.Where(x => x.Url == url);
                if (!files.Any())
                {
                    _log4.Debug("No files found for deletion");
                    return false;
                }
                dbContext.Files.RemoveRange(files);
                dbContext.SaveChanges();
                _log4.Debug("All file deleted");
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }
        public IHtmlFile GetHtmlFile(string url)
        {
            try
            {
                var file = dbContext.Files.FirstOrDefault(x => x.Url == url);
                _log4.Debug("Html files red");

                return file;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return null;
            }
        }
        public IQueryable<IHtmlFile> GetHtmlFiles()
        {
            try
            {
                _log4.Debug("All Html files red");
                return dbContext.Files;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return null;
            }
        }
        public bool InsertOrUpdateHtmlFile(IHtmlFile file)
        {
            try
            {
                using (var tempContext = new DatabaseContext(_connectionString))
                {
                    var existingFile = tempContext.Files.FirstOrDefault(x => x.Url == file.Url);
                    if (existingFile != null)
                    {
                        existingFile.Content = file.Content;
                        existingFile.LastModified = file.LastModified;
                        _log4.Debug("Existing html modified: " + existingFile.Url);

                    }
                    else
                    {
                        tempContext.Files.Add(Adapters.GetHtmlFileEntity(file));
                        _log4.Debug("New html added: " + file.Url);
                    }
                    tempContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return false;
            }
            return true;
        }

        public void RemoveDuplicateHtmlFiles()
        {
            var dupes = dbContext.Files.GroupBy(x => x.Content);

            foreach(var dupe in dupes)
            {
                dbContext.Files.RemoveRange(dupe.Skip(1));
            }
            dbContext.SaveChanges();
        }

        #endregion
    }
}
