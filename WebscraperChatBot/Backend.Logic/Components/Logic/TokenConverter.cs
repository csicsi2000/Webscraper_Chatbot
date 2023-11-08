using General.Interfaces.Backend.Logic;
using Iveonik.Stemmers;
using NTextCat;
using System.Text.RegularExpressions;

namespace Backend.Logic.Components.Logic
{
    public class TokenConverter : ITokenConverter
    {
        IList<string> _stopWords;

        public TokenConverter(IList<string> stopWords)
        {
            _stopWords = stopWords;
        }

        public IList<string> ConvertToTokens(string text)
        {
            string pattern = "[()\\.,;:%\"?!+]";

            string result = Regex.Replace(text, pattern, "");
            string[] tokens = result.Split(' ');
            IList<string> normalizedTokens = new List<string>();

            foreach (var token in tokens)
            {
                string normalizedText = NormalizeText(token);
                if (normalizedText != null)
                {
                    normalizedTokens.Add(normalizedText);
                }
            }

            // Stemming
            //using (var hunspell = new Hunspell("Resources/Hunspell/hu/index.aff", "Resources/Hunspell/hu/index.dic"))
            //{
            //    for (int i = 0; i < normalizedTokens.Count; i++)
            //    {
            //        var characters = hunspell.Stem(normalizedTokens[i]);
            //        normalizedTokens[i] = string.Join("", characters);
            //    }
            //}
            var factory = new RankedLanguageIdentifierFactory();
            var identifier = factory.Load("Resources/TextCat/Wiki82.profile.xml"); // can be an absolute or relative path. Beware of 260 chars limitation of the path length in Windows. Linux allows 4096 chars.
            var languages = identifier.Identify(result);
            var mostCertainLanguage = languages.FirstOrDefault();

            IStemmer stemmer;
            if (mostCertainLanguage?.Item1.Iso639_2T == "hu")
            {
                stemmer = new HungarianStemmer();
            }
            else
            {
                stemmer = new EnglishStemmer();
            }
            for (int i = 0; i < normalizedTokens.Count; i++)
            {
                var characters = stemmer.Stem(normalizedTokens[i]);
                normalizedTokens[i] = string.Join("", characters);
            }

            return normalizedTokens;
        }


        private string NormalizeText(string word)
        {
            word = word.Trim();
            word = word.ToLowerInvariant();

            // stop words
            if (_stopWords.Contains(word))
            {
                return null;
            }

            return word;
        }
    }
}
