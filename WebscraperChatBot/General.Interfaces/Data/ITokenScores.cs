using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces.Data
{
    public interface ITokenScore
    {
        int Id { get; }
        double Score { get; }
    }
}
