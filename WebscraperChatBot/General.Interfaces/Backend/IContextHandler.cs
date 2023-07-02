using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend
{
    public interface IContextHandler
    {
        public IEnumerable<IContext> FindContext();

        public void ExtractContext(string input);
    }
}
