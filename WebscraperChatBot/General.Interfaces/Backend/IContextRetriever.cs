using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Backend
{
    /// <summary>
    /// Get relevant contexts
    /// </summary>
    public interface IContextRetriever
    {
        /// <summary>
        /// Calculate context scores
        /// </summary>
        /// <param name="contexts"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public void CalculateContextScores(IEnumerable<IContext> contexts, string text);
    }
}
