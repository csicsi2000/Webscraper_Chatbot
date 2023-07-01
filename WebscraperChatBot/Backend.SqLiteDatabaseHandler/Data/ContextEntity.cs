using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DatabaseHandler.Data
{
    public class ContextEntity : IContext
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public int Rank { get; set; }

        public string OriginUrl { get; set; }
    }
}
