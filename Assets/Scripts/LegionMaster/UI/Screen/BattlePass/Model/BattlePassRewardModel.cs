using System;
using LegionMaster.BattlePass.Model;

namespace LegionMaster.UI.Screen.BattlePass.Model
{
    public enum BattlePassRewardViewState
    {
        Available,
        Unavailable,     
        PremiumLocked,
        Taken,
        NoReward
    }

    public class BattlePassRewardModel
    {
        public readonly BattlePassReward BattlePassReward;
        public readonly BattlePassRewardViewState ViewState;

        public BattlePassRewardModel(BattlePassReward battlePassReward, bool activePremium)
        {
            BattlePassReward = battlePassReward;
            ViewState = GetViewState(battlePassReward, activePremium);
        }
        private BattlePassRewardViewState GetViewState(BattlePassReward battlePassReward, bool activePremium)
        {
            return battlePassReward.State switch
            {
                    BattlePassRewardState.Available => BattlePassRewardViewState.Available,
                    BattlePassRewardState.Unavailable => GetLockReason(battlePassReward, activePremium),
                    BattlePassRewardState.Taken => BattlePassRewardViewState.Taken,
                    BattlePassRewardState.NoReward => BattlePassRewardViewState.NoReward,
                    _ => throw new ArgumentOutOfRangeException(nameof(battlePassReward.State), battlePassReward.State, null)
            };
        }
        private BattlePassRewardViewState GetLockReason(BattlePassReward battlePassReward, bool activePremium)
        {
            if (battlePassReward.Id.Type == BattlePassRewardType.Basic || activePremium) {
                return BattlePassRewardViewState.Unavailable;
            }
            return BattlePassRewardViewState.PremiumLocked;
        }
    }
}