using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend
{
    public interface IHtmlFileHandler
    {
        public bool InsertHtmlFile(IHtmlFile file);
        public IHtmlFile GetHtmlFile(string url);
        public bool DeleteHtmlFile(string url);
        public IEnumerable<IHtmlFile> GetHtmlFiles();

    }
}
