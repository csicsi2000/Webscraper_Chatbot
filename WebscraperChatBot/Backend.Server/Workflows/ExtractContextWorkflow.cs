using Backend.DatabaseHandler;
using Backend.WebScraper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using General.Interfaces.Backend;
using Backend.Logic.Components;

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
