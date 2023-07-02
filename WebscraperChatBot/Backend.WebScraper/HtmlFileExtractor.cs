using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.WebScraper.Data;
using General.Interfaces.Backend;
using General.Interfaces.Data;
using HtmlAgilityPack;

namespace Backend.WebScraper
{
    public class HtmlFileExtractor : IWebScraper
    {
        public IEnumerable<IHtmlFile> GetHtmlFiles(string url)
        {
            var htmlFiles = new List<IHtmlFile>();
            var baseUri = new Uri(url);
            var visitedUrls = new HashSet<string>();

            ExtractHtmlFiles(url, htmlFiles, visitedUrls, baseUri);

            return htmlFiles;
        }

        private void ExtractHtmlFiles(string url, List<IHtmlFile> htmlFiles, HashSet<string> visitedUrls, Uri baseUri)
        {
            if (visitedUrls.Contains(url))
            { 
                return; 
            }

            visitedUrls.Add(url);

            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var htmlFile = new HtmlFile
                {
                    Url = url,
                    LastModified = DateTime.Now,
                    Content = doc.DocumentNode.OuterHtml
                };

                htmlFiles.Add(htmlFile);

                foreach (var link in doc.DocumentNode.Descendants("a"))
                {
                    var href = link.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(href))
                    {
                        var absoluteUri = new Uri(baseUri, href);
                        var absoluteUrl = absoluteUri.ToString();
                        if (absoluteUri.Host == baseUri.Host && !visitedUrls.Contains(absoluteUrl))
                        {
                            ExtractHtmlFiles(absoluteUrl, htmlFiles, visitedUrls, baseUri);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                Console.WriteLine($"An error occurred while extracting HTML file from {url}: {ex.Message}");
            }

        }
    }
}
