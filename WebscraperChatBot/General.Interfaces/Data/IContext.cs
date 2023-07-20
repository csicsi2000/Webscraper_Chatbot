using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface IContext
    {
        string DocTitle { get; set; }
        string Text { get; set; }
        string OriginUrl { get; set; }
        int Rank { get; set; } 
    }
}
