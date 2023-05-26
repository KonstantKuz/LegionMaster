using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Enemy.Service;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Squad.Service;
using LegionMaster.UI.Screen.BattleMode.Model;
using LegionMaster.Units.Config;
using LegionMaster.Util;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad.Model
{
    public class SquadScreenModel
    {
        private readonly PlayerSquadService _playerSquadService;
        private readonly IReadOnlyCollection<PlacedUnitModel> _placedUnits;
        private readonly Dictionary<string, ReactiveProperty<UnitButtonModel>> _unitButtons;
        private readonly UnitCollectionConfig _unitCollectionConfig;
        
        public readonly EnemyLevelModel EnemyLevel;
        public IObservable<bool> CanStartBattle => _placedUnits.ObserveEveryValueChanged(it => 
                                                                                                 it.Any(unitModel => unitModel.IsPlaced));
        public IObservable<string> PlacedUnitCountText =>
                _playerSquadService.BattleModeUnitCount.Select(it => $"{it}/{_playerSquadService.MaxPlacedUnitCount}");
        public IEnumerable<IObservable<UnitButtonModel>> UnitButtons => _unitButtons.Values.Select(it => it);

        public SquadScreenModel(IReadOnlyCollection<PlacedUnitModel> placedUnits, 
                                InventoryService inventoryService,
                                PlayerSquadService playerSquadConfig,
                                EnemySquadService enemySquad,
                                Action<PlacedUnitModel> onClick,
                                UnitCollectionConfig unitCollectionConfig)
        {
            _placedUnits = placedUnits;
            _playerSquadService = playerSquadConfig;
            _unitCollectionConfig = unitCollectionConfig;
            EnemyLevel = EnemyLevelModel.CreateForBattle(enemySquad);
            _unitButtons = UnitButtonListModel.BuildButtonModelDictionary( 
                inventoryService.UnlockedUnitIds, 
                unitId => onClick(GetPlacedUnit(unitId)),
                IconPath.GetUnitVertical,
                null,
                LoadUnitFactionIcons);
            UpdateModel();
        }

        private List<Sprite> LoadUnitFactionIcons(string unitId)
        {
            var unitCfg = _unitCollectionConfig.GetUnitConfig(unitId);
            return unitCfg.RankConfig.Fractions
                .Select(IconPath.GetFraction)
                .Select(Resources.Load<Sprite>).ToList();
        }
        
        public void UpdateModel()
        {
            UpdateButtonsState();
        }
        private PlacedUnitModel GetPlacedUnit(string unitId)
        {
            return _placedUnits.FirstOrDefault(it => it.Id == unitId)
                   ?? throw new NullReferenceException($"PlacedUnitModel not found by unitId:= {unitId}");
        }
        private void UpdateButtonsState()
        {
            foreach (var placedUnit in _placedUnits) {
                var property = _unitButtons[placedUnit.Id];
                property.Value.CheckMark = placedUnit.IsPlaced;
                property.Value.Enabled = property.Value.Interactable = !(IsArenaFull && !property.Value.CheckMark);
                property.SetValueAndForceNotify(property.Value);
            }
        }
        private bool IsArenaFull => _placedUnits.Count(it => it.IsPlaced) >= _playerSquadService.MaxPlacedUnitCount;
    }
}