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
        public IEnumerable<IContext> GetContexts()
        {
            try
            {
                _log4.Info("All context read");
                return dbContext.Contexts;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return Enumerable.Empty<IContext>();
            }
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
                        _log4.Info("Existing context updated");
                    }
                    else
                    {
                        var contextEntity = Adapters.GetContextEntity(context);
                        contextEntity.FileEntity = tempContext.Files.FirstOrDefault(x => x.Url == contextEntity.OriginUrl);
                        tempContext.Contexts.Add(contextEntity);
                        _log4.Info("New context added");
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
                _log4.Info("All Html files red");
                return dbContext.Files;
            }
            catch (Exception ex)
            {
                _log4.Error(ex);
                return Enumerable.Empty<IHtmlFile>();
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
                        _log4.Info("Existing html modified: " + existingFile.Url);

                    }
                    else
                    {
                        tempContext.Files.Add(Adapters.GetHtmlFileEntity(file));
                        _log4.Info("New html added: " + file.Url);
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
    }
}
