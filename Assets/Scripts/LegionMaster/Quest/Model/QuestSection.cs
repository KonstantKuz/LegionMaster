using System.Collections.Generic;
using System.Linq;
using LegionMaster.Quest.Config;

namespace LegionMaster.Quest.Model
{
    public struct QuestSection
    {
        public enum RewardState
        {
            Active,
            Completed,
            RewardTaken
        }

        public struct Reward
        {
            public QuestSectionRewardConfig Config;
            public bool RewardTaken;
        }
        
        public QuestSectionType Type;
        public readonly int Points;
        public IReadOnlyList<Reward> Rewards;

        public QuestSection(QuestSectionType type, int points, IEnumerable<Reward> rewards)
        {
            Type = type;
            Points = points;
            Rewards = rewards.ToList();
        }

        public Reward? FindReward(int points)
        {
            Reward? rez = Rewards.FirstOrDefault(it => it.Config.RequiredPoints == points);
            return rez.Value.Config == null ? null : rez;
        }

        public int MaxPoints => Rewards.Select(it => it.Config.RequiredPoints).Max();
        
        public RewardState GetRewardState(Reward reward)
        {
            if (reward.RewardTaken) return RewardState.RewardTaken;
            if (reward.Config.RequiredPoints <= Points) return RewardState.Completed;
            return RewardState.Active;
        }

        public bool HasUntakenRewards()
        {
            var section = this;
            return Rewards.Any(reward => section.GetRewardState(reward) == RewardState.Completed);    
        }    
        public int UntakenRewardCount()
        {
            var section = this;
            return Rewards.Count(reward => section.GetRewardState(reward) == RewardState.Completed);    
        }
    }
}