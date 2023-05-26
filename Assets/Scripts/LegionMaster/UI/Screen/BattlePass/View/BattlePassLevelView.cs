using LegionMaster.UI.Screen.BattlePass.Model;
using UnityEngine;

namespace LegionMaster.UI.Screen.BattlePass.View
{
    public class BattlePassLevelView : MonoBehaviour
    {
        [SerializeField] private GameObject _transitionLine;
        [SerializeField] private GameObject _unavailableImage;
        [SerializeField] private BattlePassRewardView _basicReward;
        [SerializeField] private BattlePassLevelInfoView _levelInfo;
        [SerializeField] private BattlePassRewardView _premiumReward;
        public void Init(BattlePassLevelModel levelModel)
        {
            _transitionLine.SetActive(levelModel.LevelInfo.Available);
            _unavailableImage.SetActive(!levelModel.LevelInfo.Available);
          
            CreateRewardView(levelModel, levelModel.BasicReward, _basicReward);
            CreateLevelInfoView(levelModel.LevelInfo);
            CreateRewardView(levelModel, levelModel.PremiumReward, _premiumReward);
        }
        private void CreateLevelInfoView(LevelInfoModel levelInfo)
        {
            _levelInfo.Init(levelInfo);
        }
        private void CreateRewardView(BattlePassLevelModel levelModel, BattlePassRewardModel reward, BattlePassRewardView rewardView)
        {
            rewardView.Init(reward, () => levelModel.TakeRewardAction.Invoke(new TakenRewardModel(reward.BattlePassReward.Id, rewardView.DroppingLootPosition)));
        }
    }
}