namespace General.Interfaces.General
{
    public interface IConfiguration
    {
        string WebsiteRoot { get; set; }
        string WaitedClassToLoad { get; set; }
    }
}
