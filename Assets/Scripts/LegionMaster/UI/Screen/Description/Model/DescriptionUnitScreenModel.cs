using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Squad.Faction.Model;
using LegionMaster.Units.Model;
using LegionMaster.Units.Model.Meta;
using LegionMaster.UpgradeUnit.Model;
using LegionMaster.UpgradeUnit.Service;
using LegionMaster.Util;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Description.Model
{
    public class DescriptionUnitScreenModel
    {
        private readonly InventoryService _inventoryService;
        private readonly UnitModelBuilder _unitModelBuilder;
        private readonly UnitUpgradableStateProvider _upgradableStateProvider;

        private readonly ReactiveProperty<UnitDescriptionModel> _unitDescription;
        private readonly ReactiveProperty<ProgressButtonModel> _progressButtonCraftModel;
        private readonly ReactiveProperty<ProgressButtonModel> _progressButtonLevelModel;

        public IReadOnlyReactiveProperty<UnitDescriptionModel> UnitDescription => _unitDescription; 
        public IReadOnlyReactiveProperty<ProgressButtonModel> ProgressButtonCraftModel => _progressButtonCraftModel; 
        public IReadOnlyReactiveProperty<ProgressButtonModel> ProgressButtonLevelModel => _progressButtonLevelModel;
        
        private ReactiveProperty<IReadOnlyCollection<FactionItemModel>> _factions;
        public IReadOnlyReactiveProperty<IReadOnlyCollection<FactionItemModel>> Factions => _factions;
        public DescriptionUnitScreenModel(string unitId,
                                          InventoryService inventoryService,
                                          UnitModelBuilder unitModelBuilder,
                                          UnitUpgradableStateProvider upgradableStateProvider, 
                                          Action<string, RectTransform> onClickFaction)
        {
            _inventoryService = inventoryService;
            _unitModelBuilder = unitModelBuilder;
            _upgradableStateProvider = upgradableStateProvider;
            
            var model = GetUnitModel(unitId);
            var upgradeState = _upgradableStateProvider.Get(unitId);
            _unitDescription = new ReactiveProperty<UnitDescriptionModel>(BuildUnitDescriptionModel(model));
            _progressButtonCraftModel = new ReactiveProperty<ProgressButtonModel>(BuildProgressButtonCraftModel(model, upgradeState));
            _progressButtonLevelModel = new ReactiveProperty<ProgressButtonModel>(BuildProgressButtonLevelModel(model, upgradeState));
            _factions = new ReactiveProperty<IReadOnlyCollection<FactionItemModel>>(model.RankedUnit.Fractions.Select(it => BuildFactionItemModel(it, onClickFaction)).ToList());
        }

        private static ProgressButtonModel BuildProgressButtonLevelModel(UnitModel unitModel, UnitUpgradableState upgradableState)
        {
            return new ProgressButtonModel {
                    Enabled = upgradableState.IsUnlocked,
                    Interactable = upgradableState.CanUpgradeLevel,
                    CurrentValue = unitModel.InventoryUnit.Fragments,
                    MaxValue = upgradableState.LevelUpgradePrice,
            };
        }

        private UnitModel GetUnitModel(string unitId)
        {
            return _inventoryService.FindUnit(unitId) ?? _unitModelBuilder.BuildInitialUnit(unitId);
        }

        private static ProgressButtonModel BuildProgressButtonCraftModel(UnitModel unitModel,
            UnitUpgradableState upgradableState)
        {
            return new ProgressButtonModel {
                Enabled = !upgradableState.IsUnlocked,
                Interactable = upgradableState.CanCraft,
                CurrentValue = unitModel.InventoryUnit.Fragments,
                MaxValue = upgradableState.CraftPrice,
            };
        }

        private UnitDescriptionModel BuildUnitDescriptionModel(UnitModel unitModel)
        {
            return new UnitDescriptionModel {
                    UnitId = unitModel.UnitId,
                    Model = unitModel
            };
        }

        public void RebuildAll()
        {
            RebuildPriceCraftModel();
            RebuildPriceLevelModel();
            RebuildUnitDescriptionModel();
        }

        private void RebuildPriceCraftModel()
        {
            var unitId = _unitDescription.Value.UnitId;
            var model = BuildProgressButtonCraftModel(GetUnitModel(unitId), _upgradableStateProvider.Get(unitId));
            _progressButtonCraftModel.SetValueAndForceNotify(model);
        }
        public void RebuildPriceLevelModel()
        {
            var unitId = _unitDescription.Value.UnitId;
            var model = BuildProgressButtonLevelModel(GetUnitModel(unitId), _upgradableStateProvider.Get(unitId));
            _progressButtonLevelModel.SetValueAndForceNotify(model);
        }

        private void RebuildUnitDescriptionModel()
        {
            var unit = BuildUnitDescriptionModel(GetUnitModel(_unitDescription.Value.UnitId));
            _unitDescription.SetValueAndForceNotify(unit);
        }
        private FactionItemModel BuildFactionItemModel(string factionId, Action<string, RectTransform> onClickFaction)
        {
            return FactionItemModel.Create(factionId, "", true, onClickFaction);
        }
    }
}