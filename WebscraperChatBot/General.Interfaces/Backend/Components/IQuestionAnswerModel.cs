using General.Interfaces.Data;

namespace General.Interfaces.Backend.Components
{
    public interface IQuestionAnswerModel
    {
        IAnswer AnswerFromContext(string context, string question);
    }
}
