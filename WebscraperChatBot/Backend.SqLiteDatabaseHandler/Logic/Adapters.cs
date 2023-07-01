using Backend.DatabaseHandler.Data;
using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Rank = context.Rank
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
