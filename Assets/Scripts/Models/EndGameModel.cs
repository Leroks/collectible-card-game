using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Models
{
    public class EndGameModel
    {
        public Faction WinnerFaction;

        public bool AmIWinner => WinnerFaction == Faction.US;

        public int GainedXP;

        public int GainedETH;

        public int CurrentRank = 125;

        public int MaxRank = 2000;
    }
}
