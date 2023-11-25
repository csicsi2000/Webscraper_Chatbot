using Backend.Logic.Data.Json;
using General.Interfaces.Backend.Components;
using General.Interfaces.Data;
using log4net;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            client.BaseAddress = new Uri(URL);

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
                var answerObj = JsonSerializer.Deserialize<ModelPythonApi.FlaskAnswer>(responseContent);

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
