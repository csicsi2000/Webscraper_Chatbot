using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Backend.WebScraper.Data;
using General.Interfaces.Backend;
using General.Interfaces.Data;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Backend.WebScraper
{
    public class HtmlFileExtractor : IWebScraper, IDisposable
    {
        ChromeDriver _driver;
        public HtmlFileExtractor(bool withUi = false)
        {
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
            var baseUri = new Uri(url);
            var visitedUrls = new HashSet<string>();

            foreach (var htmlFile in ExtractHtmlFiles(url, visitedUrls, baseUri))
            {
                yield return htmlFile;
            }
        }

        private IEnumerable<IHtmlFile> ExtractHtmlFiles(string url, HashSet<string> visitedUrls, Uri baseUri)
        {
            if (visitedUrls.Contains(url))
            {
                yield break;
            }

            visitedUrls.Add(url);
            IHtmlFile htmlFile;


            _driver.Navigate().GoToUrl(url);
            WaitForPageLoad(_driver);

            htmlFile = new HtmlFile
            {
                Url = url,
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
            try
            {
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("main-top")));
            }
            catch
            {
                
            }

            // wait.Until(ExpectedConditions.ElementIsVisible(By.Id("myElement")));
        }
    }
}
