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
            // Step 1: Preprocess the input text to obtain the query terms
            //var queryTerms = question.Split(' ');
            var queryTerms = _tokenConverter.ConvertToTokens(question);

            // Step 2: Calculate the TF-IDF scores for the query terms
            var tfidfScores = CalculateTFIDF(contexts);

            // Step 3: Calculate the relevance score for each context
            /*
            foreach (var context in contexts)
            {
                var contextTerms = context.Text.Split(' ');
                double score = 0.0;

                foreach (var term in queryTerms)
                {
                    if (tfidfScores.ContainsKey(term) && tfidfScores[term].ContainsKey(context.DocTitle))
                    {
                        score += tfidfScores[term][context.DocTitle];
                    }
                }

               context.Score = score;
            }
            */

            Parallel.ForEach(contexts, context =>
            {
                var contextTerms = context.Text.Split(' ');
                double score = 0.0;

                foreach (var term in queryTerms)
                {
                    if (tfidfScores.ContainsKey(term) && tfidfScores[term].ContainsKey(context.DocTitle))
                    {
                        score += tfidfScores[term][context.DocTitle];
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
        Dictionary<string, Dictionary<string, double>> CalculateTFIDF(IEnumerable<IContext> contexts)
        {
            var termFrequency = new Dictionary<string, Dictionary<string, double>>();
            var documentFrequency = new Dictionary<string, int>();
            var totalDocuments = contexts.Count();

            // Step 1: Calculate term frequency (TF) and document frequency (DF) for each term in each document
            foreach (var context in contexts)
            {
                //var terms = context.Text.Split(' ').Distinct(); // Remove duplicate terms in the document // TODO
                var terms = context.Tokens.Distinct(); // Remove duplicate terms in the document // TODO
                var docTitle = context.DocTitle;

                foreach (var term in terms)
                {
                    if (!termFrequency.ContainsKey(term))
                    {
                        termFrequency[term] = new Dictionary<string, double>();
                        documentFrequency[term] = 0;
                    }

                    if (!termFrequency[term].ContainsKey(docTitle))
                    {
                        termFrequency[term][docTitle] = 0;
                        documentFrequency[term]++;
                    }

                    termFrequency[term][docTitle]++;
                }
            }

            // Step 2: Calculate inverse document frequency (IDF) for each term
            var tfidf = new Dictionary<string, Dictionary<string, double>>();

            foreach (var term in termFrequency.Keys)
            {
                tfidf[term] = new Dictionary<string, double>();

                var termDocumentFrequency = documentFrequency[term];
                var inverseDocumentFrequency = Math.Log(totalDocuments / (double)termDocumentFrequency);

                foreach (var document in termFrequency[term].Keys)
                {
                    var termFreq = termFrequency[term][document];
                    var normalizedTermFreq = termFreq / contexts.Count();
                    tfidf[term][document] = normalizedTermFreq * inverseDocumentFrequency;
                }
            }

            return tfidf;
        }
    }
}
