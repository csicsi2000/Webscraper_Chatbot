using General.Interfaces.Data;

namespace Backend.Logic.Data.Json
{
    public class ModelPythonApi
    {
        public class FlaskRequest
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
