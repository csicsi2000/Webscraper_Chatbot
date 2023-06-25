using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Data
{
    public interface IAdvancedMessage
    {
        string Text { get; }
        string SourceUrl { get; }
        string Context { get; }
        int Relevancy { get; }
        IList<IAdvancedMessage> OtherMessages { get; }
    }
}
