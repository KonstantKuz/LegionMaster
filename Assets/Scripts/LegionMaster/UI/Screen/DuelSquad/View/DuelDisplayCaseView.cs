using System;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.DuelSquad.DisplayCase;
using LegionMaster.UI.Screen.DuelSquad.Model;
using UniRx;
using LegionMaster.Extension;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.View
{
    public class DuelDisplayCaseView : MonoBehaviour
    {
        [SerializeField] private DisplayCaseUnitView _unitView; 
        [SerializeField] private AnimatedIntView _tokenAmountView;
        [SerializeField] private PriceView _shopUpdatePrice;

        private CompositeDisposable _disposable;
        private DisplayCaseItemPositions _positions;

        public void Init(IReactiveProperty<DisplayCaseUnitCollectionModel> displayedUnits, IObservable<int> tokenAmount, PriceModel shopUpdatePrice, DisplayCaseItemPositions positions)
        {
            _positions = positions;
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _unitView.Init(displayedUnits, positions);
            InitTokenAmount(tokenAmount);
            InitUpdatePrice(shopUpdatePrice);
        }
        private void InitTokenAmount(IObservable<int> tokenAmount)
        {
            _tokenAmountView.transform.position = _positions.TokenAmountPosition.WorldToScreenPoint();
            tokenAmount.Subscribe(it => _tokenAmountView.SetData(it)).AddTo(_disposable);
        }
        private void InitUpdatePrice(PriceModel model)
        {
            _shopUpdatePrice.transform.position = _positions.ShopUpdatePricePosition.WorldToScreenPoint();
            model.Available.Subscribe(available => _shopUpdatePrice.gameObject.SetActive(available)).AddTo(_disposable);
            _shopUpdatePrice.Init(model);
       
        }
        private void OnDisable()
        {
            _tokenAmountView.Reset();
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}