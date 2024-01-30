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

        string _url;
        public QuestionAnswerApiComponent(string url) 
        {
            _url = url;
        }

        public IAnswer AnswerFromContext(string context, string question)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);
            client.BaseAddress = new Uri(_url + "/interference");

            var requestModel = new FlaskPythonApi.QuestionRequest()
            {
                question = question,
                context = context
            };

            var json = JsonSerializer.Serialize(requestModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resultTask = client.PostAsync("", content);
            HttpResponseMessage response = resultTask.Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                // Parse the JSON response if needed
                var answerObj = JsonSerializer.Deserialize<FlaskPythonApi.FlaskAnswer>(responseContent, new JsonSerializerOptions
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
