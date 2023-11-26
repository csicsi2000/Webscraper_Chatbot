using General.Interfaces.Data;

namespace General.Interfaces.Backend.Components
{
    public interface IDatabaseHandler
    {
        /// <summary>
        /// Insert Html file data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool InsertOrUpdateHtmlFile(IHtmlFile file);
        /// <summary>
        /// Get the html file based on url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IHtmlFile GetHtmlFile(string url);
        /// <summary>
        /// Delete the html with with the specified url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool DeleteHtmlFile(string url);
        /// <summary>
        /// Get All html files
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IHtmlFile> GetHtmlFiles();
        /// <summary>
        /// Insert context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool InsertOrUpdateContext(IContext context);
        /// <summary>
        /// Delete context with the specified text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool DeleteContext(string text);
        /// <summary>
        /// Gett all contexts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IContext> GetContexts();
        /// <summary>
        /// Delete all data
        /// </summary>
        /// <returns></returns>
        public void DeleteAll();
    }
}
