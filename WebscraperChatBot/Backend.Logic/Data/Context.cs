using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logic.Data
{
    public class Context : IContext
    {
        public int Id { get; set; } = Guid.NewGuid().GetHashCode();
        public string DocTitle {get;set;}
        public string Text {get;set;}
        public string OriginUrl {get;set;}
        public double Score {get;set;}
        public IList<string> Tokens { get; set; }
    }
}
