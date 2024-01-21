using General.Interfaces.Data;

namespace Backend.Logic.Data.Json
{
    public class ServerSettings : IServerSettings
    {
        public string DbPath { get; set; } = "database.sqlite";
        // public string DbPath { get; set; } = "wiki7.sqlite";
        public string RootUrl { get; set; } = "https://uni-eszterhazy.hu";
        public string WaitedClassName { get; set; } = "main-top";
        public IList<string> ExcludedUrls { get; set; } = new List<string>() { "https://uni-eszterhazy.hu/api" };
        public string ModelURL { get; set; }
    }
}
