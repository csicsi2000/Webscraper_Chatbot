using Backend.DatabaseHandler;
using Backend.WebScraper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Backend.Server.Workflows
{
    public class ExtractContextWorkflow
    {
        const string dbPath = "database.sqlite";
        public void ExtraxtContext(string baseUrl, IList<string> excludedUrls)
        {
            var htmlFileExtractor = new HtmlFileExtractor("main-top", excludedUrls);

            var databaseHandler = new SqLiteDataBaseComponent(dbPath,true);

            foreach(var file in htmlFileExtractor.GetHtmlFiles(baseUrl))
            {
                databaseHandler.InsertOrUpdateHtmlFile(file);
            }
        }
    }
}
