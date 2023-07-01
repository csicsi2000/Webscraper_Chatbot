using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend
{
    public interface IDatabaseHandler
    {
        public bool InsertHtmlFile(IHtmlFile file);
        public IHtmlFile GetHtmlFile(string url);
        public bool DeleteHtmlFile(string url);
        public IEnumerable<IHtmlFile> GetHtmlFiles();

        public bool InsertContext(IContext context);
        public bool DeleteContext(string text);
        public IEnumerable<IContext> GetContexts();
    }
}
