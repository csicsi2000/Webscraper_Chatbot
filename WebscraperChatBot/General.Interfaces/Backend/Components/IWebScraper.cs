using General.Interfaces.Data;

namespace General.Interfaces.Backend.Components
{
    public interface IWebScraper
    {
        public void GetHtmlFiles(string baseUrl);
    }
}
