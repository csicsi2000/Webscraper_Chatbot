using Backend.Logic.Data;
using Flurl;
using General.Interfaces.Backend.Components;
using General.Interfaces.Data;
using HtmlAgilityPack;
using log4net;
using NTextCat.Commons;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.Concurrent;

namespace Backend.Logic.Components
{
    public class HtmlFileExtractorComponent : IWebScraper, IDisposable
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string _waitedClassName;
        IList<string> _excludedUrls;
        ChromeDriver _driver;

        string notFoundContent;
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
            string[] urlParts = baseUriText.Split('/');
            if (urlParts[urlParts.Length-1].Contains('.'))
            {
                int lastSlash = baseUriText.LastIndexOf('/');
                if (lastSlash != -1)
                {
                    baseUriText = baseUriText.Substring(0, lastSlash);
                }
            }
            var baseUri = new Uri(RemoveLastSlash(baseUriText));
            var visitedUrls = new ConcurrentBag<string>();
            setNotFoundPage(baseUri);

            return ExtractHtmlFiles(url, visitedUrls, baseUri).DistinctBy(x => x.Content).Where(x => x.Content != notFoundContent).ToList();
        }

        void setNotFoundPage(Uri rootUrl)
        {
            var notExistingPage = Url.Combine(rootUrl.AbsoluteUri.ToString(), Guid.NewGuid().ToString());
            notFoundContent = ExtractHtmlFiles(notExistingPage, new ConcurrentBag<string>(),rootUrl).First().Content;
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
        private IEnumerable<IHtmlFile> ExtractHtmlFiles(string url, ConcurrentBag<string> visitedUrls, Uri baseUri)
        {
            ConcurrentBag<IHtmlFile> resultFiles = new ConcurrentBag<IHtmlFile>();

            var currentUrl = new Uri(url).AbsoluteUri;

            if (visitedUrls.Contains(currentUrl))
            {
                return resultFiles;
            }
            if (!ProcessUrl(currentUrl).StartsWith(ProcessUrl(baseUri.AbsoluteUri)))
            {
                return resultFiles;

            }
            var foundExcluded = _excludedUrls.FirstOrDefault(x => currentUrl.StartsWith(x));
            if (foundExcluded != null)
            {
                _log4.Info("Skipped url: " + currentUrl);
                return resultFiles;

            }

            visitedUrls.Add(currentUrl);
            IHtmlFile htmlFile;


            _driver.Navigate().GoToUrl(RemoveLastSlash(currentUrl));
            Task.FromResult(WaitForPageLoad(_driver));


            htmlFile = new HtmlFile
            {
                Url = currentUrl,
                LastModified = DateTime.Now,
                Content = _driver.PageSource
            };

            resultFiles.Add(htmlFile);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlFile.Content);


            Parallel.ForEach(
                doc.DocumentNode.Descendants("a"),
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                link => {
                    var href = link.GetAttributeValue("href", string.Empty);
                    if (string.IsNullOrEmpty(href))
                    {
                        return;
                    }

                    var absoluteUri = new Uri(baseUri, href);
                    var absoluteUrl = absoluteUri.ToString();
                    if (absoluteUri.Host == baseUri.Host && !visitedUrls.Contains(absoluteUrl))
                    {
                        foreach (var subHtmlFile in ExtractHtmlFiles(absoluteUrl, visitedUrls, baseUri))
                        {
                            resultFiles.Add(subHtmlFile);
                        }
                    }

                    var absoluteUri2 = new Uri(Url.Combine(baseUri.ToString(),href));
                    var absoluteUrl2 = Url.Combine(baseUri.ToString(), href);
                    if (absoluteUri2.Host == baseUri.Host && !visitedUrls.Contains(absoluteUrl))
                    {
                        foreach (var subHtmlFile in ExtractHtmlFiles(absoluteUrl2, visitedUrls, baseUri))
                        {
                            resultFiles.Add(subHtmlFile);
                        }
                    }
                }
            );

            return resultFiles;
        }

        private async Task WaitForPageLoad(IWebDriver driver)
        {
            var jsExecutor = (IJavaScriptExecutor)driver;

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            try
            {
                wait.Until(driver => jsExecutor.ExecuteScript("return document.readyState").Equals("complete"));
                await Task.Delay(3000);
            }
            catch
            {
                _log4.Warn("Document ready timed out: " + driver.Url);

            }
            if (!_waitedClassName.IsNullOrEmpty())
            {

                try
                {
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName(_waitedClassName)));
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(_waitedClassName)));
                }
                catch
                {
                    _log4.Warn("Page did not have the element: " + driver.Url);
                }
            }
        }
    }
}
