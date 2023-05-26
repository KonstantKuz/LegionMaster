using System;
using LegionMaster.UI.Screen.BattlePass.Model;
using LegionMaster.UI.Screen.Description.View;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.BattlePass.View
{
    public class BattlePassExpView : MonoBehaviour
    {
        [SerializeField] private ExpProgressView _expProgressView;
        [SerializeField] private ButtonWithPrice _buyExpButton;  
        
        private CompositeDisposable _disposable;
        
        public void Init(IObservable<BattlePassExpModel> model)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            model.Subscribe(UpdateExpView).AddTo(_disposable);
        }

        private void UpdateExpView(BattlePassExpModel model)
        {
            _expProgressView.Init(model.ExpProgress);
            _buyExpButton.Init(model.PriceButton, model.BuyExpAction);
        }
        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}