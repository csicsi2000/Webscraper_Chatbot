using Backend.Logic.Components.Logic;
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
    public class HtmlFileExtractorComponent : IWebScraper
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string _waitedClassName;
        IList<string> _excludedUrls;

        string notFoundContent;
        IDatabaseHandler _dbHandler;
        /// <summary>
        /// Etract html files from a root page
        /// </summary>
        /// <param name="waitedClassName">Name of the class which Selenium will wait to load</param>
        /// <param name="excludedUrls">All url which starts with the excluded url will be ignored</param>
        /// <param name="withUi">Chrome UI visibility</param>
        public HtmlFileExtractorComponent(string waitedClassName, IList<string> excludedUrls,IDatabaseHandler dbHandler, bool withUi = false)
        {
            _waitedClassName = waitedClassName;
            _excludedUrls = excludedUrls ?? throw new ArgumentNullException(nameof(excludedUrls));
            _dbHandler = dbHandler ?? throw new ArgumentNullException(nameof(dbHandler));
     
        }

        public void GetHtmlFiles(string url)
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
            //setNotFoundPage(baseUri);

            ExtractHtmlFiles(url, visitedUrls, baseUri);//.DistinctBy(x => x.Content).Where(x => x.Content != notFoundContent).ToList();
            _dbHandler.RemoveDuplicateHtmlFiles();
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
        private void ExtractHtmlFiles(string url, ConcurrentBag<string> visitedUrls, Uri baseUri)
        {
            var currentUrl = new Uri(url).AbsoluteUri;

            if (visitedUrls.Contains(currentUrl))
            {
                return;
            }
            if (!ProcessUrl(currentUrl).StartsWith(ProcessUrl(baseUri.AbsoluteUri)))
            {
                return;

            }
            var foundExcluded = _excludedUrls.FirstOrDefault(x => currentUrl.StartsWith(x));
            if (foundExcluded != null)
            {
                _log4.Info("Skipped url: " + currentUrl);
                return;

            }

            visitedUrls.Add(currentUrl);
            IHtmlFile htmlFile;

            htmlFile = GetHtmlFile(currentUrl);
            _dbHandler.InsertOrUpdateHtmlFile(htmlFile);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlFile.Content);


            Parallel.ForEach(
                doc.DocumentNode.Descendants("a"),
                new ParallelOptions { MaxDegreeOfParallelism = 6 },
                link =>
                {
                    var href = link.GetAttributeValue("href", string.Empty);
                    if (string.IsNullOrEmpty(href))
                    {
                        return;
                    }

                    var absoluteUri = new Uri(baseUri, href);
                    var absoluteUrl = absoluteUri.ToString();
                    if (absoluteUri.Host == baseUri.Host && !visitedUrls.Contains(absoluteUrl))
                    {
                        ExtractHtmlFiles(absoluteUrl, visitedUrls, baseUri);
                    }
                    if(href.StartsWith("https://") || href.StartsWith("http://"))
                    {
                        return;
                    }
                    var absoluteUri2 = new Uri(Url.Combine(baseUri.ToString(), href));
                    var absoluteUrl2 = Url.Combine(baseUri.ToString(), href);
                    if (absoluteUri2.Host == baseUri.Host && !visitedUrls.Contains(absoluteUrl))
                    {
                        ExtractHtmlFiles(absoluteUrl2, visitedUrls, baseUri);
                    }
                }
            );

            return ;
        }

        private IHtmlFile GetHtmlFile(string currentUrl)
        {
            IHtmlFile htmlFile;
            using (var driverManager = new WebDriverManager())
            {
                var driver = driverManager.WebDriver;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                driver.Navigate().GoToUrl(RemoveLastSlash(currentUrl));
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();    
                WaitForPageLoad(driver);

                htmlFile = new HtmlFile
                {
                    Url = currentUrl,
                    LastModified = DateTime.Now,
                    Content = driver.PageSource
                };
            }

            return htmlFile;
        }

        private void WaitForPageLoad(IWebDriver driver)
        {
            var jsExecutor = (IJavaScriptExecutor)driver;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            try
            {
                wait.Until(driver => jsExecutor.ExecuteScript("return document.readyState").Equals("complete"));
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
