using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.General
{
    public interface IConfiguration
    {
        string WebsiteRoot { get; set; }
        string WaitedClassToLoad { get; set; }
    }
}
