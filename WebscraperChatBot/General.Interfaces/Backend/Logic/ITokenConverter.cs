namespace General.Interfaces.Backend.Logic
{
    public interface ITokenConverter
    {
        public IList<string> ConvertToTokens(string text);
    }
}
