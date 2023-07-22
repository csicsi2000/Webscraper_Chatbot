using General.Interfaces.Backend;
using General.Interfaces.Data;

namespace Backend.Logic.Components
{
    public class RetrieverComponent : IContextRetriever
    {
        public IList<IContext> GetBestContexts(IEnumerable<IContext> contexts, string text)
        {
            throw new NotImplementedException();
        }

        Dictionary<string, Dictionary<string, double>> CalculateTFIDF(string[] documents)
        {
            var termFrequency = new Dictionary<string, Dictionary<string, double>>();
            var documentFrequency = new Dictionary<string, int>();
            var totalDocuments = documents.Length;

            // Step 1: Calculate term frequency (TF) and document frequency (DF) for each term in each document
            foreach (var document in documents)
            {
                var terms = document.Split(' ').Distinct(); // Remove duplicate terms in the document

                foreach (var term in terms)
                {
                    if (!termFrequency.ContainsKey(term))
                    {
                        termFrequency[term] = new Dictionary<string, double>();
                        documentFrequency[term] = 0;
                    }

                    if (!termFrequency[term].ContainsKey(document))
                    {
                        termFrequency[term][document] = 0;
                        documentFrequency[term]++;
                    }

                    termFrequency[term][document]++;
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
                    var normalizedTermFreq = termFreq / documents.Length;
                    tfidf[term][document] = normalizedTermFreq * inverseDocumentFrequency;
                }
            }

            return tfidf;
        }
    }
}
