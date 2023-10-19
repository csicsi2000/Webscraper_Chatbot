using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend.Logic
{
    public interface ITokenConverter
    {
        public IList<string> ConvertToTokens(string text);
    }
}
