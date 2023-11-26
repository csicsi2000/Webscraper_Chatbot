namespace General.Interfaces.Data
{
    public interface IAdvancedMessage
    {
        string Text { get; }
        string SourceUrl { get; }
        string Context { get; }
        int Relevancy { get; }
        IList<IAdvancedMessage> OtherMessages { get; }
    }
}
