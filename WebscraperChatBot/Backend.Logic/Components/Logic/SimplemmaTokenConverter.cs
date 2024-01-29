using Backend.Logic.Data.Json;
using General.Interfaces.Backend.Logic;
using log4net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Backend.Logic.Components.Logic
{
    public class SimplemmaTokenConverter : ITokenConverter
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IList<string> _stopWords;
        string _url;

        public SimplemmaTokenConverter(IList<string> stopWords, string url)
        {
            _stopWords = stopWords;
            _url = url;
        }

        public IList<string> ConvertToTokens(string text)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_url);

            var requestModel = new FlaskPythonApi.TokenizationRequest
            {
                text = text
            };

            var json = JsonSerializer.Serialize(requestModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("/text-processing", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                // Parse the JSON response if needed
                var answerObj = JsonSerializer.Deserialize<FlaskPythonApi.TokenizationAnswer>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var resultTokens = new List<string>();
                foreach(var token in answerObj.tokens.Where(x => !_stopWords.Contains(x)))
                {
                    var tempToken = NormalizeText(token);
                    if(tempToken != null)
                    {
                        resultTokens.Add(tempToken);
                    }
                }
                return resultTokens;
            }
            else
            {
                _log4.Error("Api call failed o the flask server: " + response.RequestMessage);
                return null;
            }
        }

        private string NormalizeText(string word)
        {
            string pattern = "[()\\/.,;:%\\\"?!+-_&><\\[\\]]";

            word = Regex.Replace(word, pattern, " ");

            word = word.Trim();
            word = word.ToLowerInvariant();

            // stop words
            if (_stopWords.Contains(word))
            {
                return null;
            }
            if (string.IsNullOrEmpty(word))
            {
                return null;
            }

            return word;
        }
    }
}
