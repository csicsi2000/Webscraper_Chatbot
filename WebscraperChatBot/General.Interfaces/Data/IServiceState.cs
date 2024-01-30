using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface IServiceState
    {
        bool IsHtmlExtractionRunning { get; set; }
        bool IsContextExtractionRunning { get; set; }
    }
}
