using Backend.DatabaseHandler.Data;
using Backend.DatabaseHandler.Tests.Utils;
using General.Interfaces.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Backend.DatabaseHandler.Tests
{
    [TestClass]
    public class DataBaseComponentTests
    {
        DataBaseComponent dataBase ;

        [TestInitialize]
        public void TestInitialize()
        {
            if (File.Exists(GlobalValues.TestDBPath))
            {
                File.Delete(GlobalValues.TestDBPath);
            }
            SQLiteConnection.CreateFile(GlobalValues.TestDBPath);
            dataBase = new DataBaseComponent(GlobalValues.TestDBPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            dataBase.Dispose();
            dataBase = null;
        }

        [TestMethod]
        public void DeleteContext_DeleteExistingContext_DeleteSuccessful()
        {
            // Arrange
            var mockedContext = new Mock<IContext>();
            string text = "test text";
            mockedContext.Setup(x => x.Text).Returns(text);
            mockedContext.Setup(x => x.Rank).Returns(2);
            mockedContext.Setup(x => x.OriginUrl).Returns("test.com");

            dataBase.InsertContext(mockedContext.Object);

            // Act
            bool result = dataBase.DeleteContext(text);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, dataBase.GetContexts().Count());
        }

        [TestMethod]
        public void DeleteContext_DeleteNonExistingContext_ReturnFalse()
        {
            // Arrange
            string nonExistingText = "non-existing text";

            // Act
            bool result = dataBase.DeleteContext(nonExistingText);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetContexts_ReturnAllContexts()
        {
            // Arrange
            var mockedContext1 = new Mock<IContext>();
            mockedContext1.Setup(x => x.Text).Returns("context 1");
            mockedContext1.Setup(x => x.Rank).Returns(1);
            mockedContext1.Setup(x => x.OriginUrl).Returns("test.com");
            dataBase.InsertContext(mockedContext1.Object);

            var mockedContext2 = new Mock<IContext>();
            mockedContext2.Setup(x => x.Text).Returns("context 2");
            mockedContext2.Setup(x => x.Rank).Returns(2);
            mockedContext2.Setup(x => x.OriginUrl).Returns("test.com");
            dataBase.InsertContext(mockedContext2.Object);

            // Act
            var contexts = dataBase.GetContexts();

            // Assert
            Assert.AreEqual(2, contexts.Count());
        }

        [TestMethod]
        public void GetHtmlFile_ReturnHtmlFileByUrl()
        {
            // Arrange
            var mockedFile1 = new Mock<IHtmlFile>();
            string url1 = "http://example.com/file1.html";
            mockedFile1.Setup(x => x.Url).Returns(url1);
            mockedFile1.Setup(x => x.LastModified).Returns(new System.DateTime(2023, 1, 1));
            mockedFile1.Setup(x => x.Content).Returns("File 1 content");
            dataBase.InsertHtmlFile(mockedFile1.Object);

            var mockedFile2 = new Mock<IHtmlFile>();
            string url2 = "http://example.com/file2.html";
            mockedFile2.Setup(x => x.Url).Returns(url2);
            mockedFile2.Setup(x => x.LastModified).Returns(new System.DateTime(2023, 1, 1));
            mockedFile2.Setup(x => x.Content).Returns("File 2 content");
            dataBase.InsertHtmlFile(mockedFile2.Object);

            // Act
            var file = dataBase.GetHtmlFile(url2);

            // Assert
            Assert.IsNotNull(file);
            Assert.AreEqual(url2, file.Url);
        }
    }
}
