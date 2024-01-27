using General.Interfaces.Data;

namespace Backend.Logic.Data.Json
{
    public class FlaskPythonApi
    {
        public class TokenizationRequest
        {
            public string text { get; set;}
        }

        public class TokenizationAnswer
        {
            public List<string> tokens { get; set; }
        }
        public class QuestionRequest
        {
            public string question { get; set; }
            public string context { get; set; }
        }

        public class FlaskAnswer : IAnswer
        {
            public string Answer { get; set; }
            public double Score { get; set; }
        }
    }
}
