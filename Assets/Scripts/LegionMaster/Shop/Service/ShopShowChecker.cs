using System.Linq;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Purchase.Service;
using LegionMaster.Repository;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Data;
using LegionMaster.Tutorial;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.PurchaseInfo;
using LegionMaster.UI.Screen.Shop.Model;
using Zenject;

namespace LegionMaster.Shop.Service
{
    public class ShopShowChecker
    {
        private const int MIN_DUEL_PROGRESS_TO_SHOW_PACK = 2;
        private readonly ShopShownStateRepository _stateRepository = new ShopShownStateRepository();

        [Inject]
        private ShopCollectablesService _shopCollectablesService;
        [Inject]
        private PlayerProgressService _playerProgressService;
        [Inject]
        private DialogManager _dialogManager;
        [Inject]
        private ShopScreenConfig _shopScreenConfig;
        [Inject]
        private TutorialService _tutorialService;
        [Inject]
        private  InAppPurchaseService _inAppPurchaseService;

        public void TryShowPackOffer()
        {
            if (CanNotShowPackOffer()) {
                return;
            }
            if (!_inAppPurchaseService.IsBillingInitialized) return;
            var productIds = _shopScreenConfig.GetProductIds(ShopSectionId.SpecialOffers);
            var packId = productIds.FirstOrDefault(productId => !_shopCollectablesService.HasBoughtProduct(productId));
            if (packId == null) {
                return;
            }
            _dialogManager.Show<PurchaseInfoDialogPresenter, string>(packId);
            SavePackShownState(_playerProgressService.Progress.DuelProgress);
        }

        private bool CanNotShowPackOffer()
        {
            var duelProgress = _playerProgressService.Progress.DuelProgress;
            return duelProgress < MIN_DUEL_PROGRESS_TO_SHOW_PACK || _tutorialService.IsRunning
                   || duelProgress <= ShopShownState.LastShownPackDuelNumber;
        }

        private void SavePackShownState(int duelProgress)
        {
            var state = ShopShownState;
            state.LastShownPackDuelNumber = duelProgress;
            _stateRepository.Set(state);
        }

        private ShopShownState ShopShownState => _stateRepository.Get() ?? new ShopShownState();
    }
}