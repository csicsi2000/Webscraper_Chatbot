using Backend.Logic.Data;
using General.Interfaces.Backend;
using General.Interfaces.Data;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml;

namespace Backend.Logic.Components
{
    public class HtmlParserComponent : IHtmlParser
    {
        List<string> _commonElements;
        public HtmlParserComponent(IList<IHtmlFile> sampleHtmlFiles)
        {
            _commonElements = FindCommonElements(sampleHtmlFiles);
        }

        #region Find common elements
        List<string> FindCommonElements(IList<IHtmlFile> htmlFiles)
        {
            List<string> commonElements = new List<string>();

            if (htmlFiles.Count == 0)
            {
                Console.WriteLine("No HTML files provided.");
                return commonElements;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.Load(htmlFiles[0].Content);

            foreach (HtmlNode node in doc.DocumentNode.Descendants().Where(n => n.NodeType == HtmlNodeType.Element))
            {
                string elementName = node.Name;
                string elementContent = node.InnerHtml.Trim();

                if (!string.IsNullOrEmpty(elementContent) && !commonElements.Contains(elementName))
                {
                    bool isCommon = true;

                    for (int i = 1; i < htmlFiles.Count; i++)
                    {
                        HtmlDocument tempDoc = new HtmlDocument();
                        tempDoc.Load(htmlFiles[i].Content);

                        if (tempDoc.DocumentNode.Descendants().Any(n =>
                            n.NodeType == HtmlNodeType.Element &&
                            n.Name == elementName &&
                            n.InnerHtml.Trim() == elementContent))
                        {
                            continue;
                        }
                        else
                        {
                            isCommon = false;
                            break;
                        }
                    }

                    if (isCommon)
                    {
                        commonElements.Add(elementName);
                    }
                }
            }

            return commonElements;
        }

        string RemoveCommonElements(string htmlContent, List<string> commonElements)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            foreach (string element in commonElements)
            {
                foreach (HtmlNode node in doc.DocumentNode.Descendants().Where(n =>
                    n.NodeType == HtmlNodeType.Element &&
                    n.Name == element &&
                    n.InnerHtml.Trim() == GetCommonElementContent(htmlContent, element)))
                {
                    node.Remove();
                }
            }

            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);
                string modifiedHtml = writer.ToString();

                // Use the modified HTML string as needed
                return modifiedHtml;
            }
        }

        string? GetCommonElementContent(string htmlContent, string elementName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            HtmlNode? commonElementNode = doc.DocumentNode.Descendants()
                .FirstOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == elementName);

            return commonElementNode?.InnerHtml.Trim();
        }

        #endregion

        #region Remove base elements

        public IContext RemoveBaseNodes(IHtmlFile file)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(file.Content);

            string title = doc.DocumentNode.Descendants("title").FirstOrDefault()?.InnerText ?? "";

            // Remove script and style nodes
            doc.DocumentNode.Descendants()
                .Where(n => n.Name.Equals("script", StringComparison.OrdinalIgnoreCase) || n.Name.Equals("style", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(n => n.Remove());

            // Remove HTML comments
            doc.DocumentNode.DescendantsAndSelf()
                .Where(n => n.NodeType == HtmlNodeType.Comment)
                .ToList()
                .ForEach(x => x.Remove());

            string[] classElementsToRemove = { "cookie", "main-menu-bar", "footer", "site-switch" };

            foreach (var classElement in classElementsToRemove)
            {
                doc.DocumentNode.Descendants("div")
                    .Where(d => d.Attributes["class"]?.Value?.Contains(classElement, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList()
                    .ForEach(d => d.Remove());
            }

            string[] elementsToRemove = { "nav", "footer", "a" };
            foreach (var elementName in elementsToRemove)
            {
                doc.DocumentNode.Descendants(elementName)
                    .ToList()
                    .ForEach(e => e.Remove());
            }

            // Get the text from the body
            var body = doc.DocumentNode.SelectSingleNode("//body");
            string extractedText = RemoveTags(body.InnerHtml);

            // Remove HTML tags using regular expressions

            // Replace series of whitespace with a single space
            extractedText = Regex.Replace(extractedText, @"\s+", " ");

            if (extractedText.Contains("The page you were looking for was not found"))
            {
                return null;
            }
            
            // Print the extracted text
            var resContext = new Context()
            {
                DocTitle = title,
                Text = extractedText,
                OriginUrl = file.Url
            };

            // todo
            return resContext;
        }

        string RemoveTags(string html)
        {
            string pattern = @"<[^>]+>|&nbsp;";
            return Regex.Replace(html, pattern, " ").Trim();
        }

        #endregion

        public IContext ExtractRelevantContent(IHtmlFile htmlContent)
        {
            var result = RemoveBaseNodes(htmlContent);

            result.Text = RemoveCommonElements(result.Text, _commonElements);

            return result;
        }
    }
}