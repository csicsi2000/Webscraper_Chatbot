using General.Interfaces.Data;

namespace Frontend.BlazorWebassembly.Data
{
    public class BackendSettings : IServerSettings
    {
        public string DbPath { get; set; }
        public IList<string> ExcludedUrls { get; set; }
        public string RootUrl { get; set; }
        public string WaitedClassName { get; set; }
        public string ModelApiURL { get; set; }
    }
}
