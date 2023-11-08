using General.Interfaces.Data;

namespace Backend.Logic
{
    public interface IChatbotServices
    {
        void ExtractContexts(bool isSetup = false);
        void ExtractHtmls();
        IList<IContext> GetAdvanceAnswer(string question);
        string GetAnswer(string question);
    }
}