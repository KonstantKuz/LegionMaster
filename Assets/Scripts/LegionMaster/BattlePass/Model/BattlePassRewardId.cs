
namespace LegionMaster.BattlePass.Model
{
    public struct BattlePassRewardId
    {
        public BattlePassRewardType Type;
        public int Level;

        public BattlePassRewardId(int level, BattlePassRewardType type)
        {
            Level = level;
            Type = type;
        }

        public bool Equals(BattlePassRewardId other)
        {
            return Type == other.Type && Level == other.Level;
        }

        public override bool Equals(object obj)
        {
            return obj is BattlePassRewardId other && Equals(other);
        }
        
        public override string ToString()
        {
            return $"BattlePassRewardId = BattlePassRewardType:= {Type}, Level:= {Level}";
        }
        public override int GetHashCode()
        {
            unchecked {
                return ((int) Type * 397) ^ Level;
            }
        }
    }
}