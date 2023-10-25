using Backend.Logic.Components.Logic;
using Backend.Logic.Data;
using Backend.Logic.Data.Json;
using General.Interfaces.Backend;
using General.Interfaces.Backend.Logic;
using General.Interfaces.Data;
using HtmlAgilityPack;
using log4net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Backend.Logic.Components
{
    public class HtmlParserComponent : IHtmlParser
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal List<HtmlNode> _commonElements;
        internal ITokenConverter _tokenConverter;
        public HtmlParserComponent(IList<IHtmlFile> sampleHtmlFiles)
        {
            _commonElements = FindCommonElements(sampleHtmlFiles);
            ReadInStopwords();
        }

        void ReadInStopwords()
        {
            var files = Directory.GetFiles("Resources/StopWords");
            var stopWords = new List<string>();
            foreach (var file in files)
            {
                string text = File.ReadAllText(file);
                var words = JsonSerializer.Deserialize<StopWords>(text);
                if(words == null)
                {
                    _log4.Warn("Stopword File was not parsed. " + file);
                    continue;
                }
                stopWords.AddRange(words.Words);
            }
            _tokenConverter = new TokenConverter(stopWords);
        }

        #region Find common elements

        List<HtmlNode> FindCommonElements(IList<IHtmlFile> htmlFiles)
        {
            List<HtmlNode> commonElements = new List<HtmlNode>();

            if (htmlFiles.Count == 0)
            {
                Console.WriteLine("No HTML files provided.");
                return commonElements;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlFiles[0].Content);

            foreach (HtmlNode node in doc.DocumentNode.Descendants().Where(n => n.NodeType == HtmlNodeType.Element))
            {
                string elementContent = node.InnerHtml.Trim();

                if (!string.IsNullOrEmpty(elementContent) && !commonElements.Any(n =>
                    n.NodeType == HtmlNodeType.Element &&
                    n.InnerHtml.Trim() == elementContent))
                {
                    bool isCommon = true;

                    for (int i = 1; i < htmlFiles.Count; i++)
                    {
                        HtmlDocument tempDoc = new HtmlDocument();
                        tempDoc.LoadHtml(htmlFiles[i].Content);

                        if (tempDoc.DocumentNode.Descendants().Any(n =>
                            n.NodeType == HtmlNodeType.Element &&
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
                        commonElements.Add(node);
                    }
                }
            }

            return commonElements;
        }

        string RemoveCommonElements(string htmlContent, List<HtmlNode> commonElements)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            foreach (var element in commonElements)
            {
                foreach (HtmlNode node in doc.DocumentNode.Descendants().Where(n =>
                    n.NodeType == HtmlNodeType.Element &&
                    n.Name == element.Name &&  // Use element.Name here
                    n.InnerHtml.Trim() == GetCommonElementContent(htmlContent, element.Name)))  // Use element.Name here
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

        string GetCommonElementContent(string htmlContent, string elementName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Find all elements with the specified name (elementName)
            var elements = doc.DocumentNode.Descendants(elementName);

            // If there are no elements with the given name, return an empty string
            if (!elements.Any())
            {
                return string.Empty;
            }

            // Check if all elements with the specified name have the same inner content
            string commonContent = elements.First().InnerHtml.Trim();
            foreach (var element in elements.Skip(1))
            {
                string currentContent = element.InnerHtml.Trim();
                if (currentContent != commonContent)
                {
                    // If the inner content is not the same for all elements, return an empty string
                    return string.Empty;
                }
            }

            // If all elements have the same inner content, return that content
            return commonContent;
        }

        #endregion

        #region Remove base elements

        IContext RemoveBaseNodes(IHtmlFile file)
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

            // Get code tokens
            var tokens = _tokenConverter.ConvertToTokens(extractedText);
            
            // Print the extracted text
            var resContext = new Context()
            {
                DocTitle = title,
                Text = extractedText,
                OriginUrl = file.Url,
                Tokens = tokens.ToArray()
            };

            // todo
            return resContext;
        }
        #endregion

        #region Generate Tokens


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