using LegionMaster.Extension;
using LegionMaster.Reward.Model;

namespace LegionMaster.LootBox.Model
{
    public class LootBoxModel
    {
        private LootBoxId _id;
        private int _count;
        public LootBoxModel(LootBoxId id, int count)
        {
            _id = id;
            _count = count;
        }
        public static LootBoxModel FromReward(RewardItem rewardItem)
        {
            return new LootBoxModel(EnumExt.ValueOf<LootBoxId>(rewardItem.RewardId), rewardItem.Count);
        }

        public LootBoxId Id => _id;
        public int Count => _count;
    }
}