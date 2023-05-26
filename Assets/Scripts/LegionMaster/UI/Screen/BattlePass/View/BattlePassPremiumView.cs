using System;
using LegionMaster.UI.Screen.BattlePass.Model;
using LegionMaster.UI.Screen.Description;
using LegionMaster.UI.Screen.Description.View;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.BattlePass.View
{
    public class BattlePassPremiumView : MonoBehaviour
    {
        [SerializeField] private ButtonWithPrice _buyPremiumButton;
        
        private CompositeDisposable _disposable;
        public void Init(IObservable<BattlePassPremiumModel> premiumModel)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            premiumModel.Subscribe(UpdatePremiumView).AddTo(_disposable);
       
        }
        private void UpdatePremiumView(BattlePassPremiumModel premiumModel)
        {
            _buyPremiumButton.gameObject.SetActive(premiumModel.CanBuyPremium); 
            if (premiumModel.CanBuyPremium) {
                _buyPremiumButton.Init(premiumModel.PriceButton, premiumModel.BuyPremiumAction);
            }
        }
        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}