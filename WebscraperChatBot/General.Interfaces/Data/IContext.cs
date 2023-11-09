namespace General.Interfaces.Data
{
    public interface IContext
    {
        int Id { get; set; }
        string DocTitle { get; set; }
        string Text { get; set; }
        string OriginUrl { get; set; }
        double Score { get; set; }
        string[] Tokens { get; set; }

    }
}
