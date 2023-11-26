using General.Interfaces.Data;

namespace Frontend.BlazorWebassembly.Data
{
    public class ServiceStatus : IChatbotServiceStatus
    {
        public string HtmlExtraction { get ; set ; }
        public string ContextExtraction { get ; set ; }
        public string QuestionAnswer { get ; set ; }
        public int HtmlFileCount { get ; set ; }
        public int ContextCount { get ; set ; }
    }
}
