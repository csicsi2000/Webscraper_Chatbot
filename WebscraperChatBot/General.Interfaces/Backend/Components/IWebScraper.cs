using General.Interfaces.Data;

namespace General.Interfaces.Backend.Components
{
    public interface IWebScraper
    {
        public IEnumerable<IHtmlFile> GetHtmlFiles(string baseUrl);
    }
}
