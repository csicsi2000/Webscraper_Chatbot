using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logic.Data
{
    public class ServiceState : IServiceState
    {
        public bool IsHtmlExtractionRunning { get; set; } = false;
        public bool IsContextExtractionRunning { get; set; } = false;
    }
}
