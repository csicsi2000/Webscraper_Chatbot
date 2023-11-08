using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend.Components
{
    public interface IQuestionAnswerModel
    {
        string AnswerFromContext(string context, string question);
    }
}
