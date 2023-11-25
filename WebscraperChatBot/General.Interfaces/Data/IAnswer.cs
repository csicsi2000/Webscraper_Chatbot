using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface IAnswer
    {
        public string Answer {  get; set; }
        public double Score { get; set; }
    }
}
