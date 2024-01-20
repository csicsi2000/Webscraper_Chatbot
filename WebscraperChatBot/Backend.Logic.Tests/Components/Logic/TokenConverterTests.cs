using Backend.Logic.Components;
using Backend.Logic.Components.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logic.Tests.Components.Logic
{
    [TestClass]
    public class TokenConverterTests
    {
        [TestMethod]
        public void TC01_GetHtmlFiles_ValidUrl_ReturnsHtmlFiles()
        {
            // arrange
            var tokenConverter = new TokenConverter(new List<string>());

            // act
            var res = tokenConverter.ConvertToTokens("évnyitó ");

            Assert.AreEqual(1, res.Count);

        }
    }
}
