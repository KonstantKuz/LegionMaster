
using LegionMaster.Units.Component;

namespace LegionMaster.Campaign.Session.Messages
{
    public struct StageEndMessage
    {
        public UnitType Winner;
        public int Stage;
        public bool FinishedMatch;
        public int EnemiesKilled;
    }
}