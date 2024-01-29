using General.Interfaces.Data;

namespace General.Interfaces.Backend.Components
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
        public IList<ITokenScore> CalculateContextScores(IEnumerable<IContext> contexts, string text);
    }
}
