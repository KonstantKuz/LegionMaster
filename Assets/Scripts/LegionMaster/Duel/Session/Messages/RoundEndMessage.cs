using System.Collections.Generic;
using LegionMaster.Units.Component;

namespace LegionMaster.Duel.Session.Messages
{
    public struct RoundEndMessage
    {
        public UnitType Winner;
        public Dictionary<UnitType, int> Score;
        public bool FinishedMatch;
        public int EnemiesKilled;
    }
}