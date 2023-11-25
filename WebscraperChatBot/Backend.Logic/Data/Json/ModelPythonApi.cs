using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
