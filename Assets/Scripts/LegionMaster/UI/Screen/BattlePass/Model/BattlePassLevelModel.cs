using System;
using LegionMaster.BattlePass.Model;

namespace LegionMaster.UI.Screen.BattlePass.Model
{
    public class BattlePassLevelModel
    {
        public LevelInfoModel LevelInfo;
        public BattlePassRewardModel BasicReward;
        public BattlePassRewardModel PremiumReward;
        public Action<TakenRewardModel> TakeRewardAction;
    }
}