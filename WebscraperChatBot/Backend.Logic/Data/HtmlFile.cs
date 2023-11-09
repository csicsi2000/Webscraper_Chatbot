using General.Interfaces.Data;

namespace Backend.Logic.Data
{
    public class HtmlFile : IHtmlFile
    {
        public string Url { get; set; }
        public DateTime LastModified { get; set; }
        public string Content { get; set; }
    }
}
