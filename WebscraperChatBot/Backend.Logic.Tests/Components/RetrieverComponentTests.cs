﻿using Backend.Logic.Components;
using Backend.Logic.Data;
using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logic.Tests.Components
{
    [TestClass]
    public class RetrieverComponentTests
    {
        [TestMethod]
        public void TC01_GetBestContexts_MultipleContexts_ReturnsCorrectOrder()
        {
            // arrange
            var retriever = new RetrieverComponent();
            IList<IContext> retrievedContexts = new List<IContext>()
            {
                new Context()
                {
                    DocTitle = "Dolgozat",
                    Text = "Dolgozat 15.-én",
                },
                new Context()
                {
                    DocTitle = "Kilövés",
                    Text = "Rakéta kilővés"
                }
            };
            // act
            retriever.CalculateContextScores(retrievedContexts, "Dolgozat");
            var bestContexts = retrievedContexts.OrderByDescending(x => x.Score)
                                .ToList();
            // assert
            Assert.AreEqual(2, bestContexts.Count);
            Assert.AreEqual(0.34657359027997264, bestContexts.First().Score);

        }
    }
}