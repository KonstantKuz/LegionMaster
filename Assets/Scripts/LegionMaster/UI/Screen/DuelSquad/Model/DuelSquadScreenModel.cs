using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Duel.Config;
using LegionMaster.Duel.Session.Model;
using LegionMaster.Duel.Store;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Units.Component;
using LegionMaster.Units.Config;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.Model
{
    public class DuelSquadScreenModel
    {
        private const int DISPLAY_CASE_UNITS_COUNT = 3;

        private readonly IReadOnlyCollection<PlacedUnitModel> _placedUnits;
        private readonly ReactiveProperty<DisplayCaseUnitCollectionModel> _displayedUnits; 
        private readonly BoolReactiveProperty _timerEnabled;

        private DuelUnitStoreService _duelUnitStore;
        private WalletService _walletService;
        private DuelConfig _duelConfig;
        private UnitCollectionConfig _unitCollectionConfig;
        private DuelBattleSession _matchSession;
        private Action<DisplayCaseUnitId, GameObject> _onStartDrag;
        private Action<DisplayCaseUnitId, GameObject> _onStopDrag;

        public readonly DisplayCaseUpdateButtonModel UpdateButton;
        public readonly IObservable<int> TokenAmount;
        public readonly IObservable<RoundInfoModel> RoundInfo;
        public IObservable<bool> TimerEnabled => _timerEnabled;
        public IReactiveProperty<DisplayCaseUnitCollectionModel> DisplayedUnits => _displayedUnits;
        public DuelSquadScreenModel(IReadOnlyCollection<PlacedUnitModel> placedUnits,
                                    DuelUnitStoreService duelUnitStore,
                                    WalletService walletService,
                                    DuelConfig duelConfig,
                                    UnitCollectionConfig unitCollectionConfig,
                                    DuelBattleSession matchSession,
                                    bool timerEnabled,
                                    Action<DisplayCaseUnitId, GameObject> onStartDrag,
                                    Action<DisplayCaseUnitId, GameObject> onStopDrag,
                                    Action onShopUpdateClick)
        {
            _placedUnits = placedUnits;
            _duelUnitStore = duelUnitStore;
            _walletService = walletService;
            _duelConfig = duelConfig;
            _unitCollectionConfig = unitCollectionConfig;
            _matchSession = matchSession;
            _onStartDrag = onStartDrag;
            _onStopDrag = onStopDrag;
            _displayedUnits = new ReactiveProperty<DisplayCaseUnitCollectionModel>(BuildDisplayedUnits());
            TokenAmount = _walletService.GetMoneyAsObservable(Currency.DuelToken).AsObservable();
            UpdateButton = CreateUpdateButtonModel(onShopUpdateClick);
            _timerEnabled = new BoolReactiveProperty(timerEnabled);
            RoundInfo = _timerEnabled.Select(CreateRoundInfo);
        }

        public DisplayCaseUnitModel GetDisplayCaseUnit(DisplayCaseUnitId id) => _displayedUnits.Value.GetUnit(id);

        public void UpdateDisplayedUnits()
        {
            _displayedUnits.SetValueAndForceNotify(BuildDisplayedUnits());
        }

        public void UpdateMergeStatuses()
        {
            _displayedUnits.Value.UpdateMergeStatuses(_placedUnits);
            _displayedUnits.SetValueAndForceNotify(_displayedUnits.Value);
        }

        public void SetDisplayedUnits(IEnumerable<string> unitIds)
        {
            _displayedUnits.SetValueAndForceNotify(BuildDisplayedUnits(unitIds));
        }

        public void UpdateDisplayCaseUnit(DisplayCaseUnitId id, DisplayedUnitState state)
        {
            var model = GetDisplayCaseUnit(id);
            model.Update(state);
        }

        private DisplayCaseUpdateButtonModel CreateUpdateButtonModel(Action onShopUpdateClick) =>
                DisplayCaseUpdateButtonModel.Create(_duelConfig.ShopUpdatePrice, _walletService, _duelUnitStore, onShopUpdateClick);

        public PlacedUnitModel CreatePlacedUnit(DisplayCaseUnitId id)
        {
            return new PlacedUnitModel() {
                    Id = id.UnitId,
                    Star = id.Star,
            };
        }
        private DisplayCaseUnitCollectionModel BuildDisplayedUnits()
        {
            var unitIds = _duelUnitStore.AcquireUnits(DISPLAY_CASE_UNITS_COUNT);
            return BuildDisplayedUnits(unitIds);
        }

        private DisplayCaseUnitCollectionModel BuildDisplayedUnits(IEnumerable<string> unitIds)
        {
            return DisplayCaseUnitCollectionModel.CreateForDuel(unitIds, _placedUnits, _unitCollectionConfig, _walletService, _duelUnitStore, OnStartDrag, OnStopDrag);
        }

        private void OnStartDrag(DisplayCaseUnitId id, GameObject item)
        {
            if (!_duelUnitStore.CanBuyUnit(id.UnitId)) {
                return;
            }
            _onStartDrag?.Invoke(id, item);
        }

        private void OnStopDrag(DisplayCaseUnitId id, GameObject item)
        {
            if (!_duelUnitStore.CanBuyUnit(id.UnitId)) {
                return;
            }
            _onStopDrag?.Invoke(id, item);
        }

        private RoundInfoModel CreateRoundInfo(bool timerEnabled)
        {
            return new RoundInfoModel {
                    RoundStartTime = DateTime.Now.AddSeconds(_duelConfig.SecondsBeforeStartRound),
                    PlayerScore = _matchSession.WinRoundCount[UnitType.PLAYER].ToString(),
                    EnemyScore = _matchSession.WinRoundCount[UnitType.AI].ToString(),
                    RoundNumber = _matchSession.Round,
                    TimerEnabled = timerEnabled,
            };
        }

        public void EnsurePlayerHasEnoughTokens()
        {
            var requiredTokens = _displayedUnits.Value.Units.Sum(unit => _duelUnitStore.GetUnitPrice(unit.Id.UnitId));
            var ownedTokens = _walletService.GetMoney(Currency.DuelToken);
            if (requiredTokens > _walletService.GetMoney(Currency.DuelToken))
            {
                _walletService.Add(Currency.DuelToken, requiredTokens - ownedTokens);
            }
        }
    }
}