using General.Interfaces.Data;

namespace Backend.Logic
{
    /// <summary>
    /// Services provided by the chatbot
    /// </summary>
    public interface IChatbotServices
    {
        /// <summary>
        /// Start extraction of the contexts
        /// </summary>
        /// <param name="isSetup">if true, it will extract base nodes to filter out same part of each file</param>
        void ExtractContexts(bool isSetup = false);
        /// <summary>
        /// Read all html files recursively
        /// </summary>
        void ExtractHtmls();
        /// <summary>
        /// Get top answers with more details
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        IList<IContext> GetAdvancedAnswer(string question);
        /// <summary>
        /// Get answer to a question
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        IAnswer GetAnswer(string question);
        /// <summary>
        /// Get all extracted html file count
        /// </summary>
        /// <returns></returns>
        public int GetHtmlCount();
        /// <summary>
        /// Get all extracted context count
        /// </summary>
        /// <returns></returns>
        public int GetContextCount();
        /// <summary>
        /// Get the current settings of the server
        /// </summary>
        /// <returns></returns>
        IServerSettings GetSettings();
        /// <summary>
        /// Sets the extractor settings
        /// </summary>
        /// <returns></returns>
        void SetSettings(IServerSettings settings);
        /// <summary>
        /// Return the status of the background services, if they are running
        /// </summary>
        /// <returns></returns>
        IServiceState GetServiceState();
    }
}