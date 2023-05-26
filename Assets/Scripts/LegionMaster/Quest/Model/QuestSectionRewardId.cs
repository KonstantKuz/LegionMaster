namespace LegionMaster.Quest.Model
{
    public struct QuestSectionRewardId
    {
        public QuestSectionType Type;
        public int Points;

        public QuestSectionRewardId(QuestSectionType type, int points)
        {
            Type = type;
            Points = points;
        }

        public bool Equals(QuestSectionRewardId other)
        {
            return Type == other.Type && Points == other.Points;
        }

        public override bool Equals(object obj)
        {
            return obj is QuestSectionRewardId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Type * 397) ^ Points;
            }
        }
    }
}