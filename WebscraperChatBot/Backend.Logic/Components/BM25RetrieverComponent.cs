using Backend.Logic.Data;
using General.Interfaces.Backend.Components;
using General.Interfaces.Backend.Logic;
using General.Interfaces.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Logic.Components
{
    public class BM25RetrieverComponent : IContextRetriever
    {
        private readonly ITokenConverter _tokenConverter;
        private const double K1 = 2; // Term saturation parameter
        private const double B = 0.8; // Length normalization parameter

        public BM25RetrieverComponent(ITokenConverter tokenConverter)
        {
            _tokenConverter = tokenConverter;
        }

        public IList<ITokenScore> CalculateContextScores(IEnumerable<IContext> contexts, string question)
        {
            ConcurrentBag<ITokenScore> scores = new ConcurrentBag<ITokenScore>();

            var queryTerms = _tokenConverter.ConvertToTokens(question);
            ConcurrentBag<(int,string[])> neededContextValues = new ConcurrentBag<(int, string[])>(contexts.Select(x => ( x.Id, x.Tokens )));
            var bm25Scores = CalculateBM25(neededContextValues, queryTerms);

            Parallel.ForEach(neededContextValues, context =>
            {
                double score = 0.0;

                foreach (var term in queryTerms)
                {
                    if (bm25Scores.ContainsKey(term) && bm25Scores[term].ContainsKey(context.Item1))
                    {
                        double termFrequency = CalculateTermFrequency(context.Item2, term);
                        double numerator = termFrequency * (K1 + 1.0);
                        double denominator = termFrequency + K1 * (1.0 - B + B * context.Item2.Length / CalculateAverageDocumentLength(neededContextValues));
                        score += bm25Scores[term][context.Item1] * numerator / denominator;
                    }
                }
                scores.Add(new TokenScore(context.Item1, score));
            });

            return scores.ToList();
        }

        private Dictionary<string, Dictionary<int, double>> CalculateBM25(IEnumerable<(int, string[])> originContexts, IEnumerable<string> queryTerms)
        {
            var bm25Scores = new Dictionary<string, Dictionary<int, double>>();
            var contexts = originContexts.ToList();
            var totalDocuments = contexts.Count();

            foreach (var term in queryTerms)
            {
                bm25Scores[term] = new Dictionary<int, double>();

                var termDocumentFrequency = CalculateDocumentFrequency(contexts, term);
                var inverseDocumentFrequency = Math.Log((totalDocuments - termDocumentFrequency + 0.5) / (termDocumentFrequency + 0.5) + 1.0);

                foreach (var context in contexts)
                {
                    var termFrequency = CalculateTermFrequency(context.Item2, term);
                    var documentLength = context.Item2.Length;
                    var averageDocumentLength = CalculateAverageDocumentLength(contexts);

                    var numerator = termFrequency * (K1 + 1.0);
                    var denominator = termFrequency + K1 * (1.0 - B + B * documentLength / averageDocumentLength);
                    var bm25 = inverseDocumentFrequency * numerator / denominator;

                    bm25Scores[term][context.Item1] = bm25;
                }
            }

            return bm25Scores;
        }

        private int CalculateDocumentFrequency(IEnumerable<(int, string[])> contexts, string term)
        {
            return contexts.Count(context => context.Item2.Contains(term));
        }

        private int CalculateTermFrequency(string[] terms, string term)
        {
            return terms.Count(t => t.Equals(term, StringComparison.OrdinalIgnoreCase));
        }

        private double CalculateAverageDocumentLength(IEnumerable<(int, string[])> contexts)
        {
            return contexts.Average(context => context.Item2.Length);
        }
    }
}
