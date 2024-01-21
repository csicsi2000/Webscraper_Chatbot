namespace General.Interfaces.Backend.Logic
{
    // Converts the text to tokens that can be processed by the context ranking
    public interface ITokenConverter
    {
        public IList<string> ConvertToTokens(string text);
    }
}
