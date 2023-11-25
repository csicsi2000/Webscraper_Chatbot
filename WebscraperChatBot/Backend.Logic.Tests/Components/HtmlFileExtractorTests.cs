using Backend.Logic.Components;
using General.Interfaces.General;

namespace Backend.Logic.Tests.Components
{
    [TestClass]
    public class HtmlFileExtractorTests
    {
        [TestMethod]
        public void TC01_GetHtmlFiles_ValidUrl_ReturnsHtmlFiles()
        {
            // Arrange
            var extractor = new HtmlFileExtractorComponent("test", new List<string>());
            var url =  Path.Combine(GlobalVariables.TestLocation, "TestFiles/index.html");

            // Act
            var htmlFiles = extractor.GetHtmlFiles(url);

            // Assert
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(1, htmlFiles.Count());
            var firstFile = htmlFiles.First();
            Assert.AreEqual("<html><head>\r\n    <title>Our Funky HTML Page</title>\r\n    <meta name=\"description\" content=\"Our first page\">\r\n    <meta name=\"keywords\" content=\"html tutorial template\">\r\n</head>\r\n<body>\r\n    <p class=\"test\">Test text inside</p>\r\n    <a href=\"/test.html\">Test link</a>\r\n\r\n</body></html>", firstFile.Content);
            Assert.IsTrue(firstFile.Url.EndsWith("index.html"));
        }

        [TestMethod]
        public void TC02_GetHtmlFiles_ValidUrlWithExclude_ReturnsHtmlFiles()
        {
            // Arrange
            var extractor = new HtmlFileExtractorComponent("test", new List<string>()
            {
                Path.Combine("file://", GlobalVariables.TestLocation, "TestFiles/test.html")
            });
            var url = Path.Combine("file://", GlobalVariables.TestLocation, "TestFiles/index.html");

            // Act
            var htmlFiles = extractor.GetHtmlFiles(url);

            // Assert
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(1, htmlFiles.Count());
            var firstFile = htmlFiles.First();
            Assert.AreEqual("<html><head>\r\n    <title>Our Funky HTML Page</title>\r\n    <meta name=\"description\" content=\"Our first page\">\r\n    <meta name=\"keywords\" content=\"html tutorial template\">\r\n</head>\r\n<body>\r\n    <p class=\"test\">Test text inside</p>\r\n    <a href=\"/test.html\">Test link</a>\r\n\r\n</body></html>", firstFile.Content);
            Assert.IsTrue(firstFile.Url.EndsWith("index.html"));
        }

        /// <summary>
        /// Free website for testing webscrape
        /// </summary>
        [TestMethod]
        public void TC03_GetHtmlFiles_ValidUrlWithRealSite_ReturnsHtmlFiles()
        {
            // Arrange
            var extractor = new HtmlFileExtractorComponent("test-site", new List<string>()
            {
                Path.Combine("https://webscraper.io/test-sites/tables/tables-semantically-correct")
            });
            var url = "https://webscraper.io/test-sites/tables";

            // Act
            var htmlFiles = extractor.GetHtmlFiles(url);

            // Assert
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(4, htmlFiles.Count());
            var urls = htmlFiles.Select(x=> x.Url).ToList();
        }
    }
}