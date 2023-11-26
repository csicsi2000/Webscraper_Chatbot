using Backend.SqLiteDatabaseHandler.Data;
using General.Interfaces.Data;

namespace Backend.SqLiteDatabaseHandler.Logic
{
    internal static class Adapters
    {
        public static ContextEntity GetContextEntity(IContext context)
        {
            return new ContextEntity
            {
                OriginUrl = context.OriginUrl,
                Text = context.Text,
                Score = context.Score,
                DocTitle = context.DocTitle,
                Tokens = context.Tokens
            };
        }

        public static HtmlFileEntity GetHtmlFileEntity(IHtmlFile htmlFile) 
        {
            return new HtmlFileEntity
            {
                Content = htmlFile.Content,
                Url = htmlFile.Url,
                LastModified = htmlFile.LastModified,
            };
        }
    }
}
