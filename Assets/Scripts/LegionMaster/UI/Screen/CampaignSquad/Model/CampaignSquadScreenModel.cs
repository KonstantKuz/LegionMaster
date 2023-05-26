using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Campaign.Config;
using LegionMaster.Campaign.Session.Service;
using LegionMaster.Campaign.Store;
using LegionMaster.UI.Screen.DuelSquad.Model;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Units.Config;
using ModestTree;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.CampaignSquad.Model
{
    public class CampaignSquadScreenModel
    {
        private const int DISPLAY_CASE_UNITS_COUNT = 3;

        private readonly IReadOnlyCollection<PlacedUnitModel> _placedUnits;
        private readonly ReactiveProperty<DisplayCaseUnitCollectionModel> _displayedUnits;
        private readonly ReactiveProperty<UnitPlacingAdviceModel> _unitPlacingAdvice;
        private readonly BoolReactiveProperty _fightButtonEnabled;

        private CampaignUnitStoreService _campaignUnitStoreService;
        private UnitCollectionConfig _unitCollectionConfig;
        private CampaignSessionService _campaignSessionService;
        private CampaignConfig _campaignConfig;

        private int _alreadyPlacedUnitCount;
        private int _availableUnitCount;

        private Action<DisplayCaseUnitId, GameObject> _onStartDrag;
        private Action<DisplayCaseUnitId, GameObject> _onStopDrag;
        private Action<DisplayCaseUnitId> _onMissingUnitSpawned;

        public IReactiveProperty<DisplayCaseUnitCollectionModel> DisplayedUnits => _displayedUnits;
        public IObservable<bool> FightButtonEnabled => _fightButtonEnabled;
        public IReactiveProperty<UnitPlacingAdviceModel> UnitPlacingAdvice => _unitPlacingAdvice;
        private bool AllUnitReinforced => _alreadyPlacedUnitCount >= _availableUnitCount;

        public CampaignSquadScreenModel(IReadOnlyCollection<PlacedUnitModel> placedUnits,
                                        CampaignUnitStoreService campaignUnitStoreService,
                                        UnitCollectionConfig unitCollectionConfig,
                                        CampaignSessionService campaignSessionService,
                                        CampaignConfig campaignConfig,
                                        Action<DisplayCaseUnitId, GameObject> onStartDrag,
                                        Action<DisplayCaseUnitId, GameObject> onStopDrag,
                                        Action<DisplayCaseUnitId> onMissingUnitSpawned)
        {
            _placedUnits = placedUnits;
            _campaignUnitStoreService = campaignUnitStoreService;
            _unitCollectionConfig = unitCollectionConfig;
            _campaignSessionService = campaignSessionService;
            _campaignConfig = campaignConfig;
            _onStartDrag = onStartDrag;
            _onStopDrag = onStopDrag;
            _onMissingUnitSpawned = onMissingUnitSpawned;
            _availableUnitCount = Math.Min(_campaignConfig.GetStage(_campaignSessionService.CampaignBattleSession.Chapter, _campaignSessionService.Stage).ReinforcedUnitCount, 
                                           _campaignUnitStoreService.UnitAmount);
            _displayedUnits = new ReactiveProperty<DisplayCaseUnitCollectionModel>(BuildInitialDisplayedUnits());
            _unitPlacingAdvice = new ReactiveProperty<UnitPlacingAdviceModel>(BuildUnitPlacingAdvice());
            _fightButtonEnabled = new BoolReactiveProperty(AllUnitReinforced);
        }

        public void IncreaseReinforcedUnitCount()
        {
            ++_alreadyPlacedUnitCount;
            _unitPlacingAdvice.SetValueAndForceNotify(BuildUnitPlacingAdvice());
            _fightButtonEnabled.SetValueAndForceNotify(AllUnitReinforced);
        }

        public void SetDisplayedUnits(IEnumerable<string> unitIds)
        {
            _displayedUnits.SetValueAndForceNotify(BuildDisplayedUnits(unitIds));
        }

        public void UpdateMergeStatuses()
        {
            _displayedUnits.Value.UpdateMergeStatuses(_placedUnits);
            _displayedUnits.SetValueAndForceNotify(_displayedUnits.Value);
        }

        public void SetAvailableUnitCount(int amount)
        {
            _availableUnitCount = amount;
            _unitPlacingAdvice.SetValueAndForceNotify(BuildUnitPlacingAdvice());
            _fightButtonEnabled.SetValueAndForceNotify(AllUnitReinforced);
        }

        public DisplayCaseUnitModel GetDisplayCaseUnit(DisplayCaseUnitId id) => _displayedUnits.Value.GetUnit(id);

        public void AddMissingUnitFromStore(DisplayCaseUnitId missingId)
        {
            var unplacedUnits = _displayedUnits.Value.GetUnplacedOnGridUnit().ToList();
            var unplacedUnitsIds = unplacedUnits.Select(it => it.UnitId).ToList();

            var newUnitIds = _campaignUnitStoreService.AcquireUnits(unplacedUnitsIds, 1).ToList();
            if (newUnitIds.IsEmpty()) {
                _displayedUnits.SetValueAndForceNotify(BuildDisplayedUnits(unplacedUnitsIds));
                return;
            }
            var fullUnits = new List<DisplayCaseUnitId>(unplacedUnits) {
                    new DisplayCaseUnitId(newUnitIds.First(), missingId.PlaceId),
            };
            var fullUnitIds = fullUnits.OrderBy(it => it.PlaceId).Select(it => it.UnitId);
            _displayedUnits.SetValueAndForceNotify(BuildDisplayedUnits(fullUnitIds));
            _onMissingUnitSpawned?.Invoke(missingId);
        }

        public void UpdateDisplayCaseUnit(DisplayCaseUnitId id, DisplayedUnitState state)
        {
            var model = GetDisplayCaseUnit(id);
            model.Update(state);
        }

        public PlacedUnitModel CreatePlacedUnit(DisplayCaseUnitId id)
        {
            return new PlacedUnitModel() {
                    Id = id.UnitId,
                    Star = id.Star,
            };
        }

        private DisplayCaseUnitCollectionModel BuildInitialDisplayedUnits()
        {
            var unitIds = _campaignUnitStoreService.AcquireUnits(new List<string>(), DISPLAY_CASE_UNITS_COUNT);
            return BuildDisplayedUnits(unitIds);
        }

        private UnitPlacingAdviceModel BuildUnitPlacingAdvice()
        {
            return new UnitPlacingAdviceModel() {
                    AvailableUnitCount = _availableUnitCount,
                    AlreadyPlacedUnitCount = _alreadyPlacedUnitCount,
                    Enabled = !AllUnitReinforced
            };
        }

        private DisplayCaseUnitCollectionModel BuildDisplayedUnits(IEnumerable<string> unitIds)
        {
            return DisplayCaseUnitCollectionModel.CreateForCampaign(unitIds, _placedUnits, _unitCollectionConfig, OnStartDrag, OnStopDrag);
        }

        private void OnStartDrag(DisplayCaseUnitId id, GameObject item) => _onStartDrag?.Invoke(id, item);
        private void OnStopDrag(DisplayCaseUnitId id, GameObject item) => _onStopDrag?.Invoke(id, item);
    }
}