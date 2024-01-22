using General.Interfaces.Backend.Components;
using General.Interfaces.Backend.Logic;
using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Logic.Components
{
    public class BM25RetrieverComponent : IContextRetriever
    {
        ITokenConverter _tokenConverter;

        public BM25RetrieverComponent(ITokenConverter tokenConverter)
        {
            _tokenConverter = tokenConverter;
        }

        public void CalculateContextScores(IEnumerable<IContext> contexts, string question)
        {
            //var queryTerms = question.Split(' ');
            var queryTerms = _tokenConverter.ConvertToTokens(question);

            var bm25Scores = CalculateBM25(contexts);

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
                    if (bm25Scores.ContainsKey(term) && bm25Scores[term].ContainsKey(context.Id))
                    {
                        score += bm25Scores[term][context.Id];
                    }
                }

                context.Score = score;
            });
        }

        /// <summary>
        /// Calculates BM25 scores for each term in each document.
        /// </summary>
        /// <param name="contexts"></param>
        /// <returns>Dictionary with the term as the key, and inside of it the score for each document</returns>
        Dictionary<string, Dictionary<int, double>> CalculateBM25(IEnumerable<IContext> contexts)
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

            // Step 2: Calculate BM25 scores for each term in each document
            var bm25Scores = new Dictionary<string, Dictionary<int, double>>();
            double k1 = 1.5; // BM25 parameter, can be tuned
            double b = 0.75; // BM25 parameter, can be tuned

            foreach (var term in termFrequency.Keys)
            {
                bm25Scores[term] = new Dictionary<int, double>();

                var termDocumentFrequency = documentFrequency[term];
                var inverseDocumentFrequency = Math.Log((totalDocuments - termDocumentFrequency + 0.5) / (termDocumentFrequency + 0.5));

                foreach (var documentId in termFrequency[term].Keys)
                {
                    var termFreq = termFrequency[term][documentId];
                    var documentLength = contexts.First(c => c.Id == documentId).Tokens.Count(); // Use the length of the document in tokens
                    var averageDocumentLength = contexts.Select(c => c.Tokens.Count()).Average(); // Average document length in tokens

                    var K = k1 * ((1 - b) + b * (documentLength / averageDocumentLength));
                    var score = inverseDocumentFrequency * ((termFreq * (k1 + 1)) / (termFreq + K));

                    bm25Scores[term][documentId] = score;
                }
            }

            return bm25Scores;
        }
    }
}

