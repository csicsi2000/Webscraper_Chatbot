using General.Interfaces.Backend.Components;
using General.Interfaces.Backend.Logic;
using General.Interfaces.Data;

namespace Backend.Logic.Components
{
    public class RetrieverComponent : IContextRetriever
    {
        ITokenConverter _tokenConverter;
        public RetrieverComponent(ITokenConverter tokenConverter) 
        {
            _tokenConverter = tokenConverter;
        }
        public void CalculateContextScores(IEnumerable<IContext> contexts, string question)
        {
            //var queryTerms = question.Split(' ');
            var queryTerms = _tokenConverter.ConvertToTokens(question);

            var tfidfScores = CalculateTFIDF(contexts);

            Parallel.ForEach(contexts, context =>
            {
                //var contextTerms = context?.Text?.Split(' ');
                //if(contextTerms == null)
                //{
                //    return;
                //}
                double score = 0.0;

                foreach (var term in queryTerms)
                {
                    if (tfidfScores.ContainsKey(term) && tfidfScores[term].ContainsKey(context.Id))
                    {
                        score += tfidfScores[term][context.Id];
                    }
                }

                context.Score = score;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contexts"></param>
        /// <returns>Dictionary with the term as the key, and inside of it the each document score</returns>
        Dictionary<string, Dictionary<int, double>> CalculateTFIDF(IEnumerable<IContext> contexts)
        {
            var termFrequency = new Dictionary<string, Dictionary<int, double>>();
            var documentFrequency = new Dictionary<string, int>();
            var totalDocuments = contexts.Count();

            // Step 1: Calculate term frequency (TF) and document frequency (DF) for each term in each document
            foreach (var context in contexts)
            {
                // var terms = context.Text.Split(' ').Distinct(); 
                var terms = context.Tokens.Distinct(); // Remove duplicate terms in the document // TODO
                var docId = context.Id;

                foreach (var term in terms)
                {
                    if (!termFrequency.ContainsKey(term))
                    {
                        termFrequency[term] = new Dictionary<int, double>();
                        documentFrequency[term] = 0;
                    }

                    if (!termFrequency[term].ContainsKey(docId))
                    {
                        termFrequency[term][docId] = 0;
                        documentFrequency[term]++;
                    }

                    termFrequency[term][docId]++;
                }
            }

            // Step 2: Calculate inverse document frequency (IDF) for each term
            var tfidf = new Dictionary<string, Dictionary<int, double>>();

            foreach (var term in termFrequency.Keys)
            {
                tfidf[term] = new Dictionary<int, double>();

                var termDocumentFrequency = documentFrequency[term];
                var inverseDocumentFrequency = Math.Log(totalDocuments / (double)termDocumentFrequency);

                foreach (var documentId in termFrequency[term].Keys)
                {
                    var termFreq = termFrequency[term][documentId];
                    var normalizedTermFreq = termFreq / contexts.Count();
                    tfidf[term][documentId] = normalizedTermFreq * inverseDocumentFrequency;
                }
            }

            return tfidf;
        }
    }
}
