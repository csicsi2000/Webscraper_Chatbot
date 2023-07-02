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
            var extractor = new HtmlFileExtractor("test");
            var url = Path.Combine("file://",GlobalVariables.TestLocation, "TestFiles/index.html");

            // Act
            var htmlFiles = extractor.GetHtmlFiles(url);

            // Assert
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(2,htmlFiles.Count());
            var firstFile = htmlFiles.First();
            Assert.AreEqual("<html><head>\r\n    <title>Our Funky HTML Page</title>\r\n    <meta name=\"description\" content=\"Our first page\">\r\n    <meta name=\"keywords\" content=\"html tutorial template\">\r\n</head>\r\n<body>\r\n    <p class=\"test\">Test text inside</p>\r\n    <a href=\"/test.html\">Test link</a>\r\n\r\n</body></html>", firstFile.Content);
            Assert.IsTrue(firstFile.Url.EndsWith("index.html"));
        }

        [TestMethod]
        public void TC02_GetHtmlFiles_ValidUrl_ReturnsHtmlFiles()
        {
            // Arrange
            var extractor = new HtmlFileExtractor("test");
            var url = Path.Combine("file://", GlobalVariables.TestLocation, "TestFiles/index.html");

            // Act
            var htmlFiles = extractor.GetHtmlFiles(url);

            // Assert
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(2, htmlFiles.Count());
            var firstFile = htmlFiles.First();
            Assert.AreEqual("<html><head>\r\n    <title>Our Funky HTML Page</title>\r\n    <meta name=\"description\" content=\"Our first page\">\r\n    <meta name=\"keywords\" content=\"html tutorial template\">\r\n</head>\r\n<body>\r\n    <p class=\"test\">Test text inside</p>\r\n    <a href=\"/test.html\">Test link</a>\r\n\r\n</body></html>", firstFile.Content);
            Assert.IsTrue(firstFile.Url.EndsWith("index.html"));
        }
    }
}