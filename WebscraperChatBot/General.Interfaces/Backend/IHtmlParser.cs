using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend
{
    public interface IHtmlParser
    {
        // Extracts relevant context from an html file
        public IContext ExtractRelevantContent(IHtmlFile htmlContent);
    }
}
