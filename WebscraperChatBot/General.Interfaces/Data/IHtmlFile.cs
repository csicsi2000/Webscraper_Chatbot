using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface IHtmlFile
    {
        string Url { get; set; }
        DateTime LastModified { get; set; } 
        string Content { get; set; }
    }
}
