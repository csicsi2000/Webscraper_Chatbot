using General.Interfaces.Data;

namespace Frontend.BlazorWebassembly.Data
{
    public class AnswerContext : IContext
    {
        public int Id { get; set; }
        public string DocTitle { get; set; }
        public string Text { get; set; }
        public string OriginUrl { get; set; }
        public double Score { get; set; }
        public string[] Tokens { get; set; }
    }
}
