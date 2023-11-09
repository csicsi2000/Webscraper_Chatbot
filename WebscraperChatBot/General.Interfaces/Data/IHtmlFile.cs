namespace General.Interfaces.Data
{
    public interface IHtmlFile
    {
        string Url { get; set; }
        DateTime LastModified { get; set; } 
        string Content { get; set; }
    }
}
