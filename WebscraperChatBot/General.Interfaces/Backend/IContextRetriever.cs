using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend
{
    public interface IContextRetriever
    {
        public IList<IContext> GetBestContexts(IEnumerable<IContext> contexts, string text);
    }
}
