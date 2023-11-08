using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend.Components
{
    public interface IWebScraper
    {
        public IEnumerable<IHtmlFile> GetHtmlFiles(string baseUrl);
    }
}
