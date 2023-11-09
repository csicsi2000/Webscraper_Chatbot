namespace General.Interfaces.Backend.Components
{
    public interface IQuestionAnswerModel
    {
        string AnswerFromContext(string context, string question);
    }
}
