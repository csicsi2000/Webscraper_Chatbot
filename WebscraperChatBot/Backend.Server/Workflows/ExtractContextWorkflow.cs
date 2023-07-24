using Backend.Logic.Components;
using General.Interfaces.Backend;

namespace Backend.Server.Workflows
{
    public class ExtractContextWorkflow
    {
        IDatabaseHandler _databaseHandler;

        public ExtractContextWorkflow(IDatabaseHandler databaseHandler) 
        {
            _databaseHandler = databaseHandler ?? throw new ArgumentNullException(nameof(databaseHandler));
        }

        public void ExtractHtml(string baseUrl, IList<string> excludedUrls)
        {
            var htmlFileExtractor = new HtmlFileExtractorComponent("main-top", excludedUrls);

            foreach(var file in htmlFileExtractor.GetHtmlFiles(baseUrl))
            {
                _databaseHandler.InsertOrUpdateHtmlFile(file);
            }
        }

        public void ExtractContext()
        {
            var htmlParser = new HtmlParserComponent(_databaseHandler.GetHtmlFiles().Take(10).ToList());
        }
    }
}
