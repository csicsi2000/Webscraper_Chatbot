using General.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logic.Data
{
    public class TokenScore : ITokenScore
    {
        public TokenScore(int id, double score)
        {
            Id = id;
            Score = score;
        }

        public int Id { get; }

        public double Score { get; }
    }
}
