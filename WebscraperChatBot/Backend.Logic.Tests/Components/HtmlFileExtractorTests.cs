using Backend.Logic.Components;
using Backend.Logic.Tests.Support;
using General.Interfaces.Backend.Components;
using General.Interfaces.Data;
using Google.Protobuf.WellKnownTypes;
using Moq;

namespace Backend.Logic.Tests.Components
{
    [TestClass]
    public class HtmlFileExtractorTests
    {
        private static string PORT = "8021";
        private static string IP = "127.0.0.1";
        private static StaticSiteStarter testSite = new StaticSiteStarter(PORT, IP );

        private string RootUrl = $"http://{IP}:{PORT}";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            bool res = testSite.StartHttpServer("TestFiles");
            if (!res)
            {
                Assert.Fail("Failed to start test server.");
            }

        }

        [ClassCleanup]
        public static void Cleanup()
        {
            testSite.StopHttpServer();
        }

        [TestMethod,TestCategory("ApiTest")]
        public void TC01_GetHtmlFiles_ValidUrl_ReturnsHtmlFiles()
        {
            // Arrange
            var htmlFiles = new List<IHtmlFile>();
            var mockedDb = new Mock<IDatabaseHandler>();
            mockedDb.Setup(x => x.InsertOrUpdateHtmlFile(It.IsAny<IHtmlFile>()))
                .Callback<IHtmlFile>((x) => htmlFiles.Add(x))
                .Returns(true);
            var extractor = new HtmlFileExtractorComponent("test", new List<string>(),mockedDb.Object);
            var url =  RootUrl+"/main.html";

            // Act
            extractor.GetHtmlFiles(url);

            // Assert
            mockedDb.Verify(x => x.InsertOrUpdateHtmlFile(It.IsAny<IHtmlFile>()), Times.Exactly(2));
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(2, htmlFiles.Count());
            var firstFile = htmlFiles.First(x => x.Url.EndsWith("/test.html"));
            Assert.IsNotNull(firstFile);
            Assert.AreEqual("<html><head>\r\n    <title>Our Funky HTML Page</title>\r\n    <meta name=\"description\" content=\"Our first page\">\r\n    <meta name=\"keywords\" content=\"html tutorial template\">\r\n</head>\r\n<body>\r\n    <p>Test link found page</p>\r\n\r\n</body></html>", firstFile.Content);
        }

        [TestMethod, TestCategory("ApiTest")]
        public void TC02_GetHtmlFiles_ValidUrlWithExclude_ReturnsHtmlFiles()
        {
            // Arrange
            var htmlFiles = new List<IHtmlFile>();
            var mockedDb = new Mock<IDatabaseHandler>();
            mockedDb.Setup(x => x.InsertOrUpdateHtmlFile(It.IsAny<IHtmlFile>()))
                .Callback<IHtmlFile>((x) => htmlFiles.Add(x))
                .Returns(true);
            var extractor = new HtmlFileExtractorComponent("test", new List<string>()
            {
                $"{RootUrl}/test.html"
            },mockedDb.Object);
            var url = RootUrl + "/main.html";

            // Act
            extractor.GetHtmlFiles(url);

            // Assert
            mockedDb.Verify(x => x.InsertOrUpdateHtmlFile(It.IsAny<IHtmlFile>()), Times.Once);
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(1, htmlFiles.Count());
            var firstFile = htmlFiles.First();
            Assert.AreEqual("<html><head>\r\n    <title>Our Funky HTML Page</title>\r\n    <meta name=\"description\" content=\"Our first page\">\r\n    <meta name=\"keywords\" content=\"html tutorial template\">\r\n</head>\r\n<body>\r\n    <p class=\"test\">Test text inside</p>\r\n    <a href=\"/test.html\">Test link</a>\r\n\r\n</body></html>", firstFile.Content);
            Assert.IsTrue(firstFile.Url.EndsWith("/main.html"));
        }

        /// <summary>
        /// Free website for testing webscrape
        /// </summary>
        [TestMethod, TestCategory("ApiTest")]
        public void TC03_GetHtmlFiles_ValidUrlWithRealSite_ReturnsHtmlFiles()
        {
            // Arrange
            var htmlFiles = new List<IHtmlFile>();
            var mockedDb = new Mock<IDatabaseHandler>();
            mockedDb.Setup(x => x.InsertOrUpdateHtmlFile(It.IsAny<IHtmlFile>()))
                .Callback<IHtmlFile>((x) => htmlFiles.Add(x))
                .Returns(true);
            var extractor = new HtmlFileExtractorComponent("test-site", new List<string>()
            {
                "https://webscraper.io/test-sites/tables/tables-semantically-correct"
            },mockedDb.Object);
            var url = "https://webscraper.io/test-sites/tables";

            // Act
            extractor.GetHtmlFiles(url);

            // Assert

            mockedDb.Verify(x => x.InsertOrUpdateHtmlFile(It.IsAny<IHtmlFile>()), Times.Exactly(4));
            Assert.IsNotNull(htmlFiles);
            Assert.AreEqual(4, htmlFiles.Count());
            var urls = htmlFiles.Select(x=> x.Url).ToList();
        }
    }
}