using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.WebScraper.Data
{
    public class HtmlFile : IHtmlFile
    {
        public string Url { get; set; }
        public DateTime LastModified { get; set; }
        public string Content { get; set; }
    }
}
