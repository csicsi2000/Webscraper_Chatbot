using Backend.QuestionAnswerModel;
using General.Interfaces.Backend.Components;

namespace Backend.Logic.Data.Json
{
    public class ServerSettings
    {
        public string DbName { get; set; } = "database.sqlite";
        public string RootUrl { get; set; } = "https://uni-eszterhazy.hu";
        public string WaitedClassName { get; set; } = "main-top";
        public IList<string> ExcludedUrls { get; set; } = new List<string>() { "https://uni-eszterhazy.hu/api" };
        public IQuestionAnswerModel QAModel { get; set; } = new Python_DebertaModel("C:\\Users\\csics\\AppData\\Local\\Programs\\Python\\Python310\\python310.dll");
    }
}
