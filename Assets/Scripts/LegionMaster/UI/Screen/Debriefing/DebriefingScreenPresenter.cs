using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Analytics.Data;
using LegionMaster.Extension;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Location.Session.Model;
using LegionMaster.LootBox.Model;
using LegionMaster.LootBox.Service;
using LegionMaster.Reward.Model;
using LegionMaster.Reward.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.LootBox;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.BattlePass;
using LegionMaster.UI.Screen.Menu;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace LegionMaster.UI.Screen.Debriefing
{
    public class DebriefingScreenPresenter : BaseScreen
    {
        private const ScreenId DEBRIEFING_SCREEN = ScreenId.Debriefing;
        public static readonly string URL = BattleScreen.BATTLE_SCREEN + "/" + DEBRIEFING_SCREEN;
        public override ScreenId ScreenId => DEBRIEFING_SCREEN; 
        public override string Url => URL;

        [SerializeField] private ActionButton _closeButton;
        [SerializeField] private DebriefingView _debriefingView; 
        [SerializeField] private RectTransform _droppingLootPosition;

        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private RewardBattleResultService _rewardBattleResultService;
        [Inject] private Analytics.Analytics _analytics;
        [Inject] private IRewardApplyService _rewardApplyService;
        [Inject] private DialogManager _dialogManager;
        [Inject] private LootBoxOpeningService _lootBoxOpeningService;
        [Inject] private LocationObjectFactory _locationObjectFactory;
        [Inject] private IMessenger _messenger;

        private DebriefingScreenModel _model;
        
        private string _nextScreenUrl;

        [PublicAPI]
        public void Init(BattleSession battleSession, string nextScreenUrl)
        {
            Assert.IsTrue(battleSession.Winner.HasValue);
            var takenRewards = _rewardBattleResultService.CalculateRewards(battleSession.Winner.Value, battleSession);
            using (_analytics.SetAcquisitionProperties(DEBRIEFING_SCREEN.AnalyticsId(), ResourceAcquisitionType.Continuity)) {
                _rewardApplyService.ApplyRewards(takenRewards);
            }
            _model = new DebriefingScreenModel(battleSession.Winner.Value, takenRewards);
            _debriefingView.Init(_model);
            _nextScreenUrl = nextScreenUrl; 
            if (IsLootBoxReceived(takenRewards)) {
                OpenLootBox(takenRewards);
            }
            _closeButton.Init(OnClose);
            PublishDroppedLoot(takenRewards);
        }

        private void PublishDroppedLoot(List<RewardItem> takenRewards)
        {
            takenRewards.ForEach(it => it.TryPublishReceivedLoot(_messenger, _droppingLootPosition.position));
        }

        public void OnDisable()
        {
            _model = null;
        }

        private bool IsLootBoxReceived(List<RewardItem> takenRewards)
        {
            return takenRewards.Exists(it => it.RewardType == RewardType.LootBox);
        }

        private void OpenLootBox(IEnumerable<RewardItem> takenRewards)
        {
            using (_analytics.SetAcquisitionProperties(ResourceAcquisitionPlace.CHEST_OPEN, ResourceAcquisitionType.Continuity)) {
                var lootBox = takenRewards.First(it => it.RewardType == RewardType.LootBox);
                var lootBoxModel = LootBoxModel.FromReward(lootBox);
                var lootBoxContent = _lootBoxOpeningService.Open(lootBoxModel);

                var lootBoxInitModel = LootBoxDialogInitModel.Debriefing(lootBoxModel, lootBoxContent);
                LootBoxDialogPresenter.ShowWithOpeningAnimation(_dialogManager, lootBoxInitModel);
            }
        }
        
        public override IEnumerator Hide()
        {
            _locationObjectFactory.DestroyAllObjects();
            yield return base.Hide();
        } 
        private void OnClose()
        {
            if (_model.HasBattlePassReward()) {
                _screenSwitcher.SwitchTo(BattlePassScreenPresenter.URL, SwitchingParam.Create()
                                                                                      .SetParamForScreen(MenuScreen.MENU_SCREEN, false)
                                                                                      .SetParamForScreen(BattlePassScreenPresenter.BATTLE_PASS_SCREEN, false, _nextScreenUrl));
            }
            else {
                _screenSwitcher.SwitchTo(_nextScreenUrl);
            }
        }
    }
}