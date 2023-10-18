using General.Interfaces.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logic.Components
{
    internal class QuestionAnswerComponent : IQuestionAnswerModel
    {
        QuestionAnswerComponent() 
        {
        }

        public string AnswerFromContext(string context, string question)
        {
            throw new NotImplementedException();
        }
    }
}
