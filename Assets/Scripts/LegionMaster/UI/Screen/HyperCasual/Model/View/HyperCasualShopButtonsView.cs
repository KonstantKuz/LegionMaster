using System;
using System.Collections.Generic;
using LegionMaster.HyperCasual.Store.Data;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.UI.Screen.Description.View;
using SuperMaxim.Core.Extensions;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.HyperCasualMode.Model.View
{
    public class HyperCasualShopButtonsView : MonoBehaviour
    {
        [SerializeField]
        private ButtonWithPrice _buyMeleeButton;
        [SerializeField]
        private ButtonWithPrice _buyRangeButton;

        private CompositeDisposable _disposable;

        public void Init(Dictionary<MergeableUnitType, IObservable<PriceButtonModel>> priceButtons, Action<MergeableUnitType> onBuy)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            priceButtons.ForEach(it => {
                var button = GetButton(it.Key);
                it.Value.Subscribe(model => button.Init(model, () => onBuy?.Invoke(it.Key))).AddTo(_disposable);
            });
        }

        private ButtonWithPrice GetButton(MergeableUnitType type)
        {
            return type switch {
                    MergeableUnitType.Melee => _buyMeleeButton,
                    MergeableUnitType.Ranged => _buyRangeButton,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void SetButtonsInteractable(bool value)
        {
            _buyMeleeButton.Button.SetInteractable(value);
            _buyRangeButton.Button.SetInteractable(value);
        }
    }
}