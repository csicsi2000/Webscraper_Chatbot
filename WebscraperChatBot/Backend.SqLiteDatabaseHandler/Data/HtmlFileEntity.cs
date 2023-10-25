using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Interfaces.Data;

namespace Backend.SqLiteDatabaseHandler.Data
{
    public class HtmlFileEntity : IHtmlFile
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        public DateTime LastModified { get; set; }

        public string Content { get; set; }
        public ICollection<ContextEntity> contextEntities { get; set; } = new List<ContextEntity>();
    }
}
