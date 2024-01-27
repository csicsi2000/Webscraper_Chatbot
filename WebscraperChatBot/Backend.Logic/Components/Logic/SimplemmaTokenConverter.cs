using Backend.Logic.Data.Json;
using Backend.Logic.Data;
using General.Interfaces.Backend.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using log4net;

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

                return answerObj.tokens.Where(x => !_stopWords.Contains(x)).ToList();
            }
            else
            {
                _log4.Error("Api call failed o the flask server: " + response.RequestMessage);
                return null;
            }
        }
    }
}
