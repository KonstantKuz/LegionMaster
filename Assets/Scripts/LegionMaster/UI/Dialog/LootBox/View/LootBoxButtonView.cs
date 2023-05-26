using System;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.UI.Screen.Description.View;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Dialog.LootBox.View
{
    public class LootBoxButtonView : MonoBehaviour
    {
        [SerializeField] private ActionButton _closeButton;
        [SerializeField] private ButtonWithPrice _buyButton;
        
        private CompositeDisposable _disposable;
        public void Init(LootBoxDialogModel model, Action onCLose, Action onBuy)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _closeButton.Init(onCLose);
            model.LootBoxButtonModel.Subscribe(it => InitButtons(it, onBuy)).AddTo(_disposable);
        }
        private void InitButtons(LootBoxButtonModel model, Action onBuy)
        {
            if (model.PriceButton != null) {
                _buyButton.Init(model.PriceButton, onBuy);
            }
            CloseButtonEnabled = model.CloseButtonEnabled;
            BuyButtonEnabled = model.BuyButtonEnabled;
        }
        private bool CloseButtonEnabled
        {
            set => _closeButton.gameObject.SetActive(value);
        }
        private bool BuyButtonEnabled
        {
            set => _buyButton.gameObject.SetActive(value);
        }
        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}