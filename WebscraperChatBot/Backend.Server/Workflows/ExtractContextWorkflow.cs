﻿using Backend.Logic.Components;
using Backend.Logic.Components.Logic;
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
            var tokenConverter = new TokenConverter(new StopWordReader().GetStopwords());
            var htmlParser = new HtmlParserComponent(tokenConverter); // common elements
        }
    }
}
