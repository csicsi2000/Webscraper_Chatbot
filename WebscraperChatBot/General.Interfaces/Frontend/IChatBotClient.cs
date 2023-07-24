using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Frontend
{
    internal interface IChatBotClient
    {
        string SendMessage(string message);

        IAdvancedMessage SendAdvanceMessae(string message);
    }
}
