using System;
using System.Collections.Generic;
using EasyButtons;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.Extension;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Components;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Header
{
    public class WalletWidgetPresenter : MonoBehaviour
    {
        [Serializable]
        public struct CurrencyViewPair
        {
            public Currency Currency;
            public AnimatedIntView View;
        }
        [SerializeField] private List<CurrencyViewPair> _moneyViews;
        
        [Inject] private WalletService _walletService;
        [Inject] private Analytics.Analytics _analytics;

        private CompositeDisposable _disposable;

        private void OnEnable()
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            
            foreach (var pair in _moneyViews)
            {
                _walletService.GetMoneyAsObservable(pair.Currency)
                              .Subscribe(it => pair.View.SetData(it))
                              .AddTo(_disposable);

            }
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        [Button("Add Currency", Mode = ButtonMode.EnabledInPlayMode)]
        public void AddCurrency(Currency currency, int amount)
        {
            using (_analytics.SetAcquisitionProperties("UnityEditor.AddCurrency", ResourceAcquisitionType.Boost))
            {
                _walletService.Add(currency, amount);    
            }
        }
    }
}