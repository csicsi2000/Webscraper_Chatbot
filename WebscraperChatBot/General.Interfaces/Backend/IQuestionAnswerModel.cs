﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend
{
    public interface IQuestionAnswerModel
    {
        string AnswerFromContext(string context, string question);
    }
}
