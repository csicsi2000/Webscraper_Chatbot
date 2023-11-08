using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface IChatbotServiceStatus
    {
        string HtmlExtraction { get; set; }
        string ContextExtraction { get; set; }
        string QuestionAnswer { get; set; }
        int HtmlFileCount { get; set; }
        int ContextCount { get; set; }
    }
}
