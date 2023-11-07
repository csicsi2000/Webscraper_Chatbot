using Backend.SqLiteDatabaseHandler.Tests.Utils;
using General.Interfaces.Data;
using Moq;

namespace Backend.SqLiteDatabaseHandler.Tests
{
    [TestClass]
    public class SqLiteDataBaseComponentTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            GlobalValues.TestDatabase.DeleteAll();
        }

        [TestMethod]
        public void TC01_DeleteContext_DeleteExistingContext_DeleteSuccessful()
        {
            // Arrange
            var mockedContext = new Mock<IContext>();
            string text = "test text";
            mockedContext.Setup(x => x.Text).Returns(text);
            mockedContext.Setup(x => x.Score).Returns(2);
            mockedContext.Setup(x => x.OriginUrl).Returns("test.com");
            mockedContext.Setup(x => x.DocTitle).Returns("test");

            GlobalValues.TestDatabase.InsertOrUpdateContext(mockedContext.Object);

            // Act
            bool result = GlobalValues.TestDatabase.DeleteContext(text);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, GlobalValues.TestDatabase.GetContexts().Count());
        }

        [TestMethod]
        public void TC02_DeleteContext_DeleteNonExistingContext_ReturnFalse()
        {
            // Arrange
            string nonExistingText = "non-existing text";

            // Act
            bool result = GlobalValues.TestDatabase.DeleteContext(nonExistingText);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TC03_InsertContexts_ContextInserted()
        {
            // Arrange
            var mockedContext1 = new Mock<IContext>();
            mockedContext1.Setup(x => x.Text).Returns("context 1");
            mockedContext1.Setup(x => x.Score).Returns(1);
            mockedContext1.Setup(x => x.OriginUrl).Returns("test.com");
            mockedContext1.Setup(x => x.DocTitle).Returns("test");

            // Act
            var res = GlobalValues.TestDatabase.InsertOrUpdateContext(mockedContext1.Object);
            var contexts = GlobalValues.TestDatabase.GetContexts();

            // Assert
            Assert.IsTrue(res);
            Assert.AreEqual(1, contexts.Count());
            var firstItem = contexts.First();
            Assert.AreEqual("context 1", firstItem.Text);
            Assert.AreEqual(1, firstItem.Score);
            Assert.AreEqual("test.com", firstItem.OriginUrl);
        }

        [TestMethod]
        public void TC04_GetContexts_ReturnAllContexts()
        {
            // Arrange
            var mockedContext1 = new Mock<IContext>();
            mockedContext1.Setup(x => x.Text).Returns("context 1");
            mockedContext1.Setup(x => x.Score).Returns(1);
            mockedContext1.Setup(x => x.OriginUrl).Returns("test.com");
            mockedContext1.Setup(x => x.DocTitle).Returns("test1");
            GlobalValues.TestDatabase.InsertOrUpdateContext(mockedContext1.Object);

            var mockedContext2 = new Mock<IContext>();
            mockedContext2.Setup(x => x.Text).Returns("context 2");
            mockedContext2.Setup(x => x.Score).Returns(2);
            mockedContext2.Setup(x => x.OriginUrl).Returns("test2.com");
            mockedContext2.Setup(x => x.DocTitle).Returns("test2");
            GlobalValues.TestDatabase.InsertOrUpdateContext(mockedContext2.Object);

            // Act
            var contexts = GlobalValues.TestDatabase.GetContexts();

            // Assert
            Assert.AreEqual(2, contexts.Count());
        }

        [TestMethod]
        public void TC05_GetContexts_SamePage_Return1Contexts()
        {
            // Arrange
            var mockedContext1 = new Mock<IContext>();
            mockedContext1.Setup(x => x.Text).Returns("context 1");
            mockedContext1.Setup(x => x.Score).Returns(1);
            mockedContext1.Setup(x => x.OriginUrl).Returns("test.com");
            mockedContext1.Setup(x => x.DocTitle).Returns("test1");
            GlobalValues.TestDatabase.InsertOrUpdateContext(mockedContext1.Object);

            var mockedContext2 = new Mock<IContext>();
            mockedContext2.Setup(x => x.Text).Returns("context 2");
            mockedContext2.Setup(x => x.Score).Returns(2);
            mockedContext2.Setup(x => x.OriginUrl).Returns("test.com");
            mockedContext2.Setup(x => x.DocTitle).Returns("test2");
            GlobalValues.TestDatabase.InsertOrUpdateContext(mockedContext2.Object);

            // Act
            var contexts = GlobalValues.TestDatabase.GetContexts();

            // Assert
            Assert.AreEqual(1, contexts.Count());
        }

        [TestMethod]
        public void TC05_GetHtmlFile_ReturnHtmlFileByUrl()
        {
            // Arrange
            var mockedFile1 = new Mock<IHtmlFile>();
            string url1 = "http://example.com/file1.html";
            mockedFile1.Setup(x => x.Url).Returns(url1);
            mockedFile1.Setup(x => x.LastModified).Returns(new DateTime(2023, 1, 1));
            mockedFile1.Setup(x => x.Content).Returns("File 1 content");
            var res1 = GlobalValues.TestDatabase.InsertOrUpdateHtmlFile(mockedFile1.Object);

            var mockedFile2 = new Mock<IHtmlFile>();
            string url2 = "http://example.com/file2.html";
            mockedFile2.Setup(x => x.Url).Returns(url2);
            mockedFile2.Setup(x => x.LastModified).Returns(new DateTime(2023, 1, 1));
            mockedFile2.Setup(x => x.Content).Returns("File 2 content");
            var res2 = GlobalValues.TestDatabase.InsertOrUpdateHtmlFile(mockedFile2.Object);

            // Act
            var file = GlobalValues.TestDatabase.GetHtmlFile(url2);

            // Assert
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsNotNull(file);
            Assert.AreEqual(url2, file.Url);
        }

        [TestMethod]
        public void TC06_InsertOrUpdateHtmlFile_ExistingUrl_Updated()
        {
            // Arrange
            var mockedFile1 = new Mock<IHtmlFile>();
            string url1 = "http://example.com/file1.html";
            mockedFile1.Setup(x => x.Url).Returns(url1);
            mockedFile1.Setup(x => x.LastModified).Returns(new DateTime(2023, 1, 1));
            mockedFile1.Setup(x => x.Content).Returns("File 1 content");
            GlobalValues.TestDatabase.InsertOrUpdateHtmlFile(mockedFile1.Object);

            var mockedFile2 = new Mock<IHtmlFile>();
            string url2 = "http://example.com/file2.html";
            mockedFile2.Setup(x => x.Url).Returns(url2);
            mockedFile2.Setup(x => x.LastModified).Returns(new DateTime(2023, 1, 1));
            mockedFile2.Setup(x => x.Content).Returns("File 2 content");
            GlobalValues.TestDatabase.InsertOrUpdateHtmlFile(mockedFile2.Object);

            // Act
            mockedFile1.Setup(x => x.Content).Returns("New text");
            GlobalValues.TestDatabase.InsertOrUpdateHtmlFile(mockedFile1.Object);


            // Assert
            Assert.AreEqual(2, GlobalValues.TestDatabase.GetHtmlFiles().Count());
            var file = GlobalValues.TestDatabase.GetHtmlFile(url1);
            Assert.IsNotNull(file);
            Assert.AreEqual("New text", file.Content);
        }
    }
}
