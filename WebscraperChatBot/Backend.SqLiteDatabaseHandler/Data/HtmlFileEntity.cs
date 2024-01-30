using System.ComponentModel.DataAnnotations;
using General.Interfaces.Data;

namespace Backend.SqLiteDatabaseHandler.Data
{
    public class HtmlFileEntity : IHtmlFile
    {
        public HtmlFileEntity()
        {
            this.contextEntities = new HashSet<ContextEntity>();
        }

        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        public DateTime LastModified { get; set; }

        public string Content { get; set; }
        public virtual ICollection<ContextEntity> contextEntities { get; set; } = new List<ContextEntity>();
    }
}
