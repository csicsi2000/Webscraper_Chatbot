using Backend.Logic.Data.Json;
using General.Interfaces.Backend.Components;
using General.Interfaces.Data;
using log4net;
using System.Text;
using System.Text.Json;

namespace Backend.Logic.Components
{
    internal class QuestionAnswerApiComponent : IQuestionAnswerModel
    {
        ILog _log4 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string URL;
        public QuestionAnswerApiComponent(string url) 
        {
            URL = url;
        }

        public IAnswer AnswerFromContext(string context, string question)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(URL + "/interference");

            var requestModel = new ModelPythonApi.FlaskRequest()
            {
                question = question,
                context = context
            };

            var json = JsonSerializer.Serialize(requestModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                // Parse the JSON response if needed
                var answerObj = JsonSerializer.Deserialize<ModelPythonApi.FlaskAnswer>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return answerObj;
            }
            else
            {
                _log4.Error("Api call failed o the flask server: " + response.RequestMessage);
                return null;
            }
        }
    }
}
