using Backend.Logic.Data;
using General.Interfaces.Backend.Components;
using General.Interfaces.Data;
using HtmlAgilityPack;
using log4net;
using NTextCat.Commons;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Backend.Logic.Components
{
    public class HtmlFileExtractorComponent : IWebScraper, IDisposable
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string _waitedClassName;
        IList<string> _excludedUrls;
        ChromeDriver _driver;

        /// <summary>
        /// Etract html files from a root page
        /// </summary>
        /// <param name="waitedClassName">Name of the class which Selenium will wait to load</param>
        /// <param name="excludedUrls">All url which starts with the excluded url will be ignored</param>
        /// <param name="withUi">Chrome UI visibility</param>
        public HtmlFileExtractorComponent(string waitedClassName, IList<string> excludedUrls, bool withUi = false)
        {
            _waitedClassName = waitedClassName;
            _excludedUrls = excludedUrls ?? throw new ArgumentNullException(nameof(excludedUrls));

            var options = new ChromeOptions();
            if (!withUi)
            {
                options.AddArgument("--headless");
            }
            _driver = new ChromeDriver(options);
        }

        public void Dispose()
        {
            _driver.Close();
            _driver.Dispose();
        }

        public IEnumerable<IHtmlFile> GetHtmlFiles(string url)
        {
            string baseUriText = url;
            if (baseUriText.EndsWith(".html"))
            {
                int lastSlash = baseUriText.LastIndexOf('/');
                if(lastSlash != -1)
                {
                    baseUriText = baseUriText.Substring(0, lastSlash);
                }
            }
            var baseUri = new Uri(RemoveLastSlash(baseUriText));
            var visitedUrls = new HashSet<string>();

            foreach (var htmlFile in ExtractHtmlFiles(url, visitedUrls, baseUri))
            {
                yield return htmlFile;
            }
        }

        string ProcessUrl(string url)
        {
            return url.Replace("https", "").Replace("http", "");
        }

        string RemoveLastSlash(string url)
        {
            if (url.EndsWith("/"))
            {
                return url.Substring(0, url.Length - 1);
            }
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Currently searched URL</param>
        /// <param name="visitedUrls">Already extracted URLs</param>
        /// <param name="baseUri">The base of the url where we search</param>
        /// <returns></returns>
        private IEnumerable<IHtmlFile> ExtractHtmlFiles(string url, HashSet<string> visitedUrls, Uri baseUri)
        {
            var currentUrl = new Uri(url).AbsoluteUri;

            if (visitedUrls.Contains(currentUrl))
            {
                yield break;
            }
            if (!ProcessUrl(currentUrl).StartsWith(ProcessUrl(baseUri.AbsoluteUri)))
            {
                yield break;
            }
            var foundExcluded = _excludedUrls.FirstOrDefault(x => currentUrl.StartsWith(x));
            if (foundExcluded != null)
            {
                _log4.Info("Skipped url: " + currentUrl);
                yield break;
            }

            visitedUrls.Add(currentUrl);
            IHtmlFile htmlFile;


            _driver.Navigate().GoToUrl(RemoveLastSlash(currentUrl));
            WaitForPageLoad(_driver);

            
            htmlFile = new HtmlFile
            {
                Url = currentUrl,
                LastModified = DateTime.Now,
                Content = _driver.PageSource
            };

            yield return htmlFile;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlFile.Content);

            foreach (var link in doc.DocumentNode.Descendants("a"))
            {
                var href = link.GetAttributeValue("href", string.Empty);
                if (string.IsNullOrEmpty(href))
                {
                    continue;
                }

                var absoluteUri = new Uri(baseUri, href);
                var absoluteUrl = absoluteUri.ToString();
                if (absoluteUri.Host == baseUri.Host && !visitedUrls.Contains(absoluteUrl))
                {
                    foreach (var subHtmlFile in ExtractHtmlFiles(absoluteUrl, visitedUrls, baseUri))
                    {
                        yield return subHtmlFile;
                    }
                }
            }
        }

        private void WaitForPageLoad(IWebDriver driver)
        {
            var jsExecutor = (IJavaScriptExecutor)driver;

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            wait.Until(driver => jsExecutor.ExecuteScript("return document.readyState").Equals("complete"));
            if (!_waitedClassName.IsNullOrEmpty())
            {

                try
                {
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName(_waitedClassName)));
                }
                catch
                {
                    _log4.Warn("Page did not have the element.");
                }
            }

            // wait.Until(ExpectedConditions.ElementIsVisible(By.Id("myElement")));
        }
    }
}
