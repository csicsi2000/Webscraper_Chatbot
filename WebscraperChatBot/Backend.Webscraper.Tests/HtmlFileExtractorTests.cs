using Backend.WebScraper;
using General.Interfaces.General;
using System.Diagnostics;
using System.Reflection;

namespace Backend.Webscraper.Tests
{
    [TestClass]
    public class HtmlFileExtractorTests
    {
        [TestMethod]
        public void TC01_GetHtmlFiles_ValidUrl_ReturnsHtmlFiles()
        {
            // Arrange
            var extractor = new HtmlFileExtractor();
            var url = Path.Combine(GlobalVariables.TestLocation, "TestFiles/index.html");

            // Act
            var htmlFiles = extractor.GetHtmlFiles(url);

            // Assert
            Assert.IsNotNull(htmlFiles);
            Assert.IsTrue(htmlFiles.Any());
            Assert.IsTrue(htmlFiles.All(file => file.Url.StartsWith(url)));
            var firstFile = htmlFiles.First();
            Assert.IsNotNull("",firstFile.Content);

        }
    }
}