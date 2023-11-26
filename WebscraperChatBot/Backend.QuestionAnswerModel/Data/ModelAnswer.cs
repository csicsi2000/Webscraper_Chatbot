using General.Interfaces.Data;

namespace Backend.QuestionAnswerModel.Data
{
    internal class ModelAnswer : IAnswer
    {
        public double Score { get; set; }
        public string Answer { get; set; }
    }
}
