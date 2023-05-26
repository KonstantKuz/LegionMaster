using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.DuelSquad.Model;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.View
{
    public class PriceView : MonoBehaviour
    {
        [SerializeField] private TextView _price; 
        [SerializeField] private GameObject _notAvailableContainer;

        private CompositeDisposable _disposable;
        public void Init(PriceModel priceModel)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _price.Init(new ReactiveProperty<string>(priceModel.Price));
            priceModel.Available.Subscribe(UpdateAvailable).AddTo(_disposable);
        }
        private void UpdateAvailable(bool available)
        {
            _notAvailableContainer.SetActive(!available);
        }
        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}