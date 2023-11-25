using General.Interfaces.Data;

namespace Frontend.BlazorWebassembly.Services
{
    public class AppStateService
    {
        public IList<string> AllMessages { get; set; } = new List<string>();
        IList<KeyValuePair<string, List<IContext>>> QuestionAnswers { get; set; } = new List<KeyValuePair<string, List<IContext>>>();
    }
}
