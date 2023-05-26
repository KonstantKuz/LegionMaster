using LegionMaster.Core.Mode;
using LegionMaster.Location.Session.Model;

namespace LegionMaster.Location.Session.Messages
{
    public struct BattleEndMessage
    {
        public GameMode GameMode;
        
        public BattleResult BattleResult;
        
        public int EnemiesKilled;
        public bool IsPlayerWon => BattleResult == BattleResult.WIN;
    }
}