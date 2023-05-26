using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Core.Config;
using LegionMaster.Extension;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.DuelSquad.DisplayCase;
using LegionMaster.UI.Screen.DuelSquad.Model;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UI.Screen.Squad.UnitPlaceChanged.Message;
using SuperMaxim.Core.Extensions;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.DuelSquad.View
{
    public class DisplayCaseUnitView : MonoBehaviour
    {
        [SerializeField] private Transform _factionsRoot; 
        [SerializeField] private Transform _pricesRoot;
        [SerializeField] private Transform _upgradeStatusesRoot;
        [SerializeField] private SpriteListView _factionListPrefab;
        [SerializeField] private PriceView _pricePrefab;
        [SerializeField] private GameObject _upgradeStatusPrefab;
        
        [Inject] private DiContainer _container;
        
        private CompositeDisposable _disposableUnits;
        private CompositeDisposable _disposableView;
        private DisplayCaseItemPositions _positions;
        public void Init(IReactiveProperty<DisplayCaseUnitCollectionModel> units, DisplayCaseItemPositions positions)
        {
            gameObject.SetActive(true);
            _positions = positions;
            _disposableUnits?.Dispose();
            _disposableUnits = new CompositeDisposable();
            units.Subscribe(UpdateUnitsView).AddTo(_disposableUnits);
        }
        private void UpdateUnitsView(DisplayCaseUnitCollectionModel unitCollection)
        {
            RemoveAllCreatedObjects();

            unitCollection.Units.ForEach(it => {
                CreateFaction(it.FactionIcons, it.State, it.Id.PlaceId);
                if (it.PriceModel != null) {
                    CreatePrice(it.PriceModel, it.State, it.Id.PlaceId);
                }
                SetUnitMergeStatus(it.CanMerge, it.State, it.Id.PlaceId);
            });
        }
        private void CreateFaction(IReadOnlyCollection<Sprite> factionIcons, IObservable<DisplayedUnitState> unitState, int placeId)
        {
            if (!AppConfig.FactionEnabled) {
                return;
            }
            var factionListView = _container.InstantiatePrefabForComponent<SpriteListView>(_factionListPrefab, _factionsRoot);
            factionListView.transform.position = _positions.UnitFactionPositions[placeId].WorldToScreenPoint();
            factionListView.Init(factionIcons);
            unitState.Subscribe(it => factionListView.gameObject.SetActive(it == DisplayedUnitState.NotTaken)).AddTo(_disposableView);
        }
        private void CreatePrice(PriceModel priceModel, IObservable<DisplayedUnitState> unitState, int placeId)
        {
            var priceView = Instantiate(_pricePrefab, _pricesRoot);
            priceView.transform.position = _positions.UnitPricePositions[placeId].WorldToScreenPoint();
            priceView.Init(priceModel);
            unitState.Subscribe(it => priceView.gameObject.SetActive(it == DisplayedUnitState.NotTaken)).AddTo(_disposableView);
        }
        private void SetUnitMergeStatus(bool canMerge, IObservable<DisplayedUnitState> unitState, int placeId)
        {
            if (!canMerge)
            {
                return;
            }
            
            var upgradeView = Instantiate(_upgradeStatusPrefab, _upgradeStatusesRoot);
            upgradeView.transform.position = _positions.UnitUpgradeStatusPositions[placeId].WorldToScreenPoint();
            unitState.Subscribe(it => upgradeView.gameObject.SetActive(it == DisplayedUnitState.NotTaken)).AddTo(_disposableView);
        }
        
        private void OnDisable()
        {
            RemoveAllCreatedObjects();
            _disposableUnits?.Dispose();
            _disposableUnits = null;
        }
        private void RemoveAllCreatedObjects()
        {
            _disposableView?.Dispose();
            _disposableView = new CompositeDisposable();

            _factionsRoot.transform.DestroyAllChildren();
            if (_pricesRoot != null) {
                _pricesRoot.transform.DestroyAllChildren();
            }
            _upgradeStatusesRoot.transform.DestroyAllChildren();
        }
    }
}