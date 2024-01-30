using General.Interfaces.Data;

namespace Backend.Logic.Data
{
    public class Context : IContext
    {
        public int Id { get; set; } = Guid.NewGuid().GetHashCode();
        public string DocTitle {get;set;}
        public string Text {get;set;}
        public string OriginUrl {get;set;}
        public double Score { get; set; } = 0;
        public string[] Tokens { get; set; }
    }
}
