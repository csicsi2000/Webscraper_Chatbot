using Backend.Logic.Components;
using Backend.Logic.Data;
using General.Interfaces.Backend.Logic;
using General.Interfaces.Data;
using Moq;

namespace Backend.Logic.Tests.Components
{
    [TestClass]
    public class RetrieverComponentTests
    {
        [TestMethod]
        public void TC01_GetBestContexts_MultipleContexts_ReturnsCorrectOrder()
        {
            // arrange
            var mockedTokenConverter = new Mock<ITokenConverter>();
            mockedTokenConverter.Setup(x => x.ConvertToTokens(It.IsAny<string>())).Returns((string x) => x.Split(' ').ToList());
            var retriever = new RetrieverComponent(mockedTokenConverter.Object);
            IList<IContext> retrievedContexts = new List<IContext>()
            {
                new Context()
                {
                    Id=0,
                    DocTitle = "Dolgozat",
                    Text = "Dolgozat 15.-én",
                },
                new Context()
                {
                    Id=1,
                    DocTitle = "Kilövés",
                    Text = "Rakéta kilővés"
                }
            };

            foreach (var context in retrievedContexts)
            {
                context.Tokens = context.Text.Split(' ');
            }
            // act
            retriever.CalculateContextScores(retrievedContexts, "Dolgozat");
            var bestContexts = retrievedContexts.OrderByDescending(x => x.Score).ToList();
            // assert
            Assert.AreEqual(2, bestContexts.Count);
            Assert.AreEqual(0.34657359027997264, bestContexts.First().Score);

        }
    }
}
