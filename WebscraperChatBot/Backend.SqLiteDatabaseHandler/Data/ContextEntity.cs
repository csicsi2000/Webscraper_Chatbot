using General.Interfaces.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.SqLiteDatabaseHandler.Data
{
    public class ContextEntity : IContext
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string DocTitle { get; set; }
        public HtmlFileEntity? FileEntity { get; set; }
        public string[] Tokens { get; set; } = new string[0];
        public string OriginUrl { get; set; }

        [NotMapped]
        public double Score { get; set; }
    }
}
