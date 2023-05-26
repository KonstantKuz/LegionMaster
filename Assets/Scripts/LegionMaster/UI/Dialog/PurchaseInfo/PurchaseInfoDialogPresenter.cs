using System;
using System.Collections;
using System.Collections.Generic;
using LegionMaster.Analytics.Data;
using LegionMaster.Config;
using LegionMaster.Extension;
using LegionMaster.Purchase.Config;
using LegionMaster.Purchase.Service;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Reward.Model;
using LegionMaster.Reward.Service;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Overlay;
using LegionMaster.UI.Screen;
using SuperMaxim.Core.Extensions;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Dialog.PurchaseInfo
{
    public class PurchaseInfoDialogPresenter : BaseDialog, IUiInitializable<string>
    {
        [SerializeField]
        private PurchaseInfoView _view;
        [SerializeField]
        private float _autoCloseTime = 0.5f;

        [Inject] private Analytics.Analytics _analytics;
        [Inject] private StringKeyedConfigCollection<ProductConfig> _shopConfig;
        [Inject] private StringKeyedConfigCollection<PurchaseConfig> _purchases;
        [Inject] private PackConfigCollection _packs;
        [Inject] private ShopService _shop;
        [Inject] private InAppPurchaseService _inAppPurchaseService;
        [Inject] private IRewardApplyService _rewardApplyService;
        [Inject] private LockOverlay _lockOverlay;
        [Inject] private IMessenger _messenger;
        [Inject] private DialogManager _dialogManager;

        private PurchaseInfoModel _model;
        private CompositeDisposable _disposable;
        private Coroutine _closeAfterTimeCoroutine;
        public void Init(string productId)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _model = CreateModel(productId);
            _view.Init(_model, OnClose);
        }

        private PurchaseInfoModel CreateModel(string productId)
        {
            if (_shopConfig.Contains(productId)) {
                return new PurchaseInfoModel(productId, _shopConfig, _packs, _shop, OnBuyProduct);
            }
            if (_purchases.Contains(productId)) {
                return new PurchaseInfoModel(productId, _purchases, _packs, _inAppPurchaseService, OnPurchase);
            }
            throw new ArgumentException($"Product rewards not found, productId:= {productId}");
        }

        private void OnClose()
        {
            if (_dialogManager.IsDialogActive<PurchaseInfoDialogPresenter>()) {
                _dialogManager.Hide<PurchaseInfoDialogPresenter>();
            }
        }

        private void CloseAfterTime()
        {
            _closeAfterTimeCoroutine = StartCoroutine(CloseAfterTimeCoroutine());
        }

        private IEnumerator CloseAfterTimeCoroutine()
        {
            yield return new WaitForSeconds(_autoCloseTime);
            OnClose();
        }

        private void OnBuyProduct(string productId)
        {
            using (_analytics.SetAcquisitionProperties(ScreenId.Shop.AnalyticsId(), ResourceAcquisitionType.Boost)) {
                var rewards = _shop.TryBuy(productId) ?? throw new Exception($"Can't buy shop product, productId:= {productId}");
                ApplyRewards(rewards);
            }
            CloseAfterTime();
        }

        private void OnDisable()
        {
            if (_closeAfterTimeCoroutine != null)
            {
                StopCoroutine(_closeAfterTimeCoroutine);
                _closeAfterTimeCoroutine = null;
            }
            _model = null;
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnPurchase(string productId)
        {
            _lockOverlay.Lock();
            _inAppPurchaseService.Buy(productId, OnPurchaseSuccess, OnPurchaseError);
        }

        private void OnPurchaseSuccess(string productId)
        {
            _lockOverlay.Unlock();
            using (_analytics.SetAcquisitionProperties(ScreenId.Shop.AnalyticsId(), ResourceAcquisitionType.Boost)) {
                var rewards = _purchases.Get(productId).GetRewards(_packs);
                ApplyRewards(rewards);
            }
            CloseAfterTime();
        }

        private void OnPurchaseError(bool cancelled, string message)
        {
            _lockOverlay.Unlock();
            if (cancelled) {
                Debug.Log("Purchase canceled");
                return;
            }
            Debug.LogError($"Can't buy shop product, purchase error:= {message}");
            CloseAfterTime();
        }

        private void ApplyRewards(IEnumerable<RewardItem> rewards)
        {
            rewards.ForEach(it => {
                _rewardApplyService.ApplyReward(it);
                TryPublishReceivedLoot(it);
            });
        }

        private void TryPublishReceivedLoot(RewardItem reward)
        {
            reward.TryPublishReceivedLoot(_messenger, _view.RewardPosition(reward).position);
        }
    }
}