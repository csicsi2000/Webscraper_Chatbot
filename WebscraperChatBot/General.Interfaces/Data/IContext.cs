﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface IContext
    {
        int Id { get; set; }
        string DocTitle { get; set; }
        string Text { get; set; }
        string OriginUrl { get; set; }
        double Score { get; set; }
        IList<string> Tokens { get; set; }

    }
}
