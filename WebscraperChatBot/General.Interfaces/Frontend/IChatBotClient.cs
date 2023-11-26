using General.Interfaces.Data;

namespace General.Interfaces.Frontend
{
    internal interface IChatBotClient
    {
        string SendMessage(string message);

        IAdvancedMessage SendAdvanceMessae(string message);
    }
}
