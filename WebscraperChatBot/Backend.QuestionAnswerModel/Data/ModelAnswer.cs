using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.QuestionAnswerModel.Data
{
    internal class ModelAnswer : IAnswer
    {
        public double Score { get; set; }
        public string Answer { get; set; }
    }
}
