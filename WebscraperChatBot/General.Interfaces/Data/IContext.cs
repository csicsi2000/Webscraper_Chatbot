using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface IContext
    {
        int Rank { get; set; }
        string DocTitle { get; set; }
        string Text { get; set; }
        string OriginUrl { get; set; }
    }
}
