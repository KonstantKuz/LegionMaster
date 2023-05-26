using LegionMaster.BattlePass.Model;
using LegionMaster.Location.Session.Messages;
using LegionMaster.Repository;
using LegionMaster.UI.Screen;
using LegionMaster.UI.Screen.BattlePass;
using SuperMaxim.Messaging;

namespace LegionMaster.BattlePass.Service
{
    public class BattlePassShowChecker
    {
        private readonly BattlePassService _battlePassService;       
        private readonly BattlePassShownStateRepository _battlePassShownStateRepository;
        private readonly ScreenSwitcher _screenSwitcher;

        public BattlePassShowChecker(BattlePassService battlePassService, 
                                     BattlePassShownStateRepository battlePassShownStateRepository, 
                                     ScreenSwitcher screenSwitcher, 
                                     IMessenger messenger)
        {
            _battlePassService = battlePassService;
            _battlePassShownStateRepository = battlePassShownStateRepository;
            _screenSwitcher = screenSwitcher;
            messenger.Subscribe<BattleEndMessage>(OnBattleFinished);
        }
        public void ShowBattlePass(bool fromMenu)
        {
            _screenSwitcher.SwitchTo(BattlePassScreenPresenter.URL, false, fromMenu, null);
            SetStateShown();
        }
        public void TryShowBattlePass()
        {
            if (!_battlePassService.HasAnyAvailableRewards || BattlePassShownState.Shown) {
                return;
            }
            _screenSwitcher.SwitchTo(BattlePassScreenPresenter.URL, false, true, null);
            SetStateShown();
        }

        private void SetStateShown()
        {
            var state = BattlePassShownState;
            state.Shown = true;
            _battlePassShownStateRepository.Set(state);
        }
        private void OnBattleFinished(BattleEndMessage msg)
        {
            ResetStateShown();
        }
        private void ResetStateShown()
        {
            _battlePassShownStateRepository.Delete();
        }
        private BattlePassShownState BattlePassShownState => _battlePassShownStateRepository.Get() ?? new BattlePassShownState();
    }
}