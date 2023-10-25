using General.Interfaces.Backend;

namespace Backend.QuestionAnswerModel.Tests
{
    [TestClass]
    public class DebertaModelTests
    {
        static IQuestionAnswerModel model;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            model = new Python_DebertaModel("C:\\Users\\csics\\AppData\\Local\\Programs\\Python\\Python310\\python310.dll");
        }

        [TestMethod]
        [TestCategory("ApiTest")]
        public void TC01_EnAnswerFromContext_CorrectDataGiven_AnswerReturned()
        {
            // arrange
            string context = @"
            🤗 Transformers is backed by the three most popular deep learning libraries — Jax, PyTorch and TensorFlow — with a seamless integration
            between them. It's straightforward to train your models with one before loading them for inference with the other.
            ";
            string question = "Which deep learning libraries back 🤗 Transformers?";

            // act
            string res = model.AnswerFromContext(context,question);

            // assert
            Assert.AreEqual("Jax, PyTorch and TensorFlow",res);
        }

        [TestMethod]
        [TestCategory("ApiTest")]
        [DataRow("Ki Volt András?", "András egy fizikus votl", "fizikus")]
        public void TC02_EnAnswerFromContext_CorrectDataGiven_AnswerReturned(string question, string context, string answer)
        {
            // arrange

            // act
            string res = model.AnswerFromContext(context, question);

            // assert
            Assert.AreEqual(answer, res);
        }
    }
}