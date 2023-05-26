using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.Session.Model;
using LegionMaster.Reward.Model;

namespace LegionMaster.UI.Screen.Debriefing
{
    public class DebriefingScreenModel
    {
        private const string REWARD_COUNT_PREFIX = "+";
        
        private readonly BattleResult _battleResult;
        private readonly List<RewardItemModel> _rewards;     
        private readonly List<RewardItem> _takenRewards;

        public DebriefingScreenModel(BattleResult battleResult, List<RewardItem> takenRewards)
        {
            _battleResult = battleResult;
            _takenRewards = takenRewards;
            _rewards = takenRewards.Select(BuildRewardItemModel).ToList();
        }

        private RewardItemModel BuildRewardItemModel(RewardItem rewardItem)
        {
            return RewardItemModel.CreateForDebriefing(REWARD_COUNT_PREFIX, rewardItem);
        }
        public bool HasBattlePassReward() => TakenRewards.FirstOrDefault(it => it.RewardType == RewardType.BattlePassExp) != null;
        public BattleResult BattleResult => _battleResult;
        public IReadOnlyCollection<RewardItemModel> RewardsItemModels => _rewards;  
        public IReadOnlyCollection<RewardItem> TakenRewards => _takenRewards;
      

    }
}