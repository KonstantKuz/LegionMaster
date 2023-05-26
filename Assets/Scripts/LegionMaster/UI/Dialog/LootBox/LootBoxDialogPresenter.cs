using System;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.LootBox.Model;
using LegionMaster.LootBox.Service;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.UI.Dialog.LootBox.View;
using UnityEngine;
using Zenject;
using UnityEngine.Assertions;

namespace LegionMaster.UI.Dialog.LootBox
{
    public class LootBoxDialogPresenter : BaseDialog, IUiInitializable<LootBoxDialogInitModel>
    {
        [SerializeField] private LootBoxRewardView _rewardView;
        [SerializeField] private LootBoxButtonView _buttonView;
        [SerializeField] private TextMeshProLocalization _caption;

        [Inject] private DialogManager _dialogManager;
        [Inject] private LootBoxOpeningService _lootBoxOpening;
        [Inject] private ShopService _shop;      
        [Inject] private Analytics.Analytics _analytics;

        private LootBoxDialogModel _model;
        public void Init(LootBoxDialogInitModel initModel)
        {
            _model = new LootBoxDialogModel(initModel, _shop);
            _buttonView.Init(_model, OnCloseButton, OnBuyButton);                
            _rewardView.Init(_model);
            _caption.LocalizationId = initModel.Caption;
        }
        
        private void OnBuyButton()
        {
            using (_analytics.SetAcquisitionProperties(ResourceAcquisitionPlace.CHEST_OPEN, ResourceAcquisitionType.Boost)) {
                var shopLootBoxId = _model.InitModel.ShopLootBoxId;
                var lootBox = _shop.TryBuyLootBox(shopLootBoxId) ?? throw new Exception($"Can't buy loot box, id= {shopLootBoxId}");
                var dialogInitModel = LootBoxDialogInitModel.Shop(lootBox, shopLootBoxId, _lootBoxOpening.Open(lootBox)); 
                ShowWithOpeningAnimation(_dialogManager, dialogInitModel);
            }
        }

        private void OnDisable()
        {
            _model = null;
        }

        private void OnCloseButton()
        {
            _dialogManager.Hide<LootBoxDialogPresenter>();
        }

        public static void ShowWithOpeningAnimation(DialogManager dialogManager, LootBoxDialogInitModel model)
        {
            Assert.IsTrue(model.LootBox != null, "Lootbox not set, can't open");
            var lootBoxOpeningInitModel = new LootBoxOpeningInitModel(model.LootBox.Id, () =>
            {
                dialogManager.Hide<LootBoxOpeningDialogPresenter>();
                dialogManager.Show<LootBoxDialogPresenter, LootBoxDialogInitModel>(model);
            });
            dialogManager.Show<LootBoxOpeningDialogPresenter, LootBoxOpeningInitModel>(lootBoxOpeningInitModel);
        }
    }
}