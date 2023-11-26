using General.Interfaces.Data;

namespace General.Interfaces.Backend.Components
{
    public interface IHtmlParser
    {
        // Extracts relevant context from an html file
        public IContext ExtractRelevantContent(IHtmlFile htmlContent);
    }
}
