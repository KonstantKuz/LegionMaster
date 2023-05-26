using JetBrains.Annotations;
using LegionMaster.Reward.Model;

namespace LegionMaster.BattlePass.Model
{
    public class BattlePassReward
    {
        public BattlePassRewardId Id { get; }
        [CanBeNull] public RewardItem Reward { get; }
        public BattlePassRewardState State { get; }

        public BattlePassReward(BattlePassRewardId id, RewardItem reward, BattlePassRewardState state)
        {
            Id = id;
            Reward = reward;
            State = state;
        }

        public bool Equals(BattlePassReward other)
        {
            return Id.Equals(other.Id) && Reward.Equals(other.Reward) && State == other.State;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BattlePassReward) obj);
        }

        public override int GetHashCode()
        {
            unchecked {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Reward != null ? Reward.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) State;
                return hashCode;
            }
        }
    }
}