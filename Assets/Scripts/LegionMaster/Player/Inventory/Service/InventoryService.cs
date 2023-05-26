using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Analytics.Data;
using LegionMaster.Analytics.Event;
using LegionMaster.Player.Inventory.Message;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Player.Progress.Service;
using LegionMaster.ProgressUnit.Config;
using LegionMaster.Repository;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model;
using LegionMaster.Units.Model.Meta;
using SuperMaxim.Messaging;
using Zenject;

namespace LegionMaster.Player.Inventory.Service
{
    public class InventoryService
    {
        [Inject]
        private InventoryRepository _inventoryRepository;
        [Inject]
        private UnitCollectionConfig _unitCollectionConfig;    
        [Inject]
        private UnitModelBuilder _unitModelBuilder;
        [Inject] 
        private UnitUnlockConfig _unitUnlockConfig;
        [Inject]
        private Analytics.Analytics _analytics;
        [Inject]
        private IMessenger _messenger;
        [Inject] 
        private PlayerProgressService _playerProgress;
        [Inject] 
        private PlayerProgressService _playerProgressService;
        
        private List<UnitModel> _playerUnits;

        private void InitUnitState(string unitId)
        {
            var inventory = Inventory;
            if (inventory.ContainsUnit(unitId)) {
                throw new ArgumentException($"Unit already exist in inventory, unitId:= {unitId}");
            }
            var unitConfig = _unitCollectionConfig.GetUnitConfig(unitId);
            var unit = InventoryUnit.FromConfig(unitConfig);
            inventory.AddUnit(unit);
            PlayerUnits.Add(_unitModelBuilder.BuildUnit(unit.UnitId, unit, null));
            _inventoryRepository.Set(inventory);
        }

        public void LoadUnit(string unitId, int level, int stars)
        {
            GetOrCreateUnit(unitId);
            var unit = FindUnit(unitId);
            unit.InventoryUnit.Star = Math.Max(stars, unit.RankedUnit.StartingStars);
            unit.InventoryUnit.Level = level;
            _inventoryRepository.Set(Inventory);
        }

        public void ResetInventory()
        {
            _inventoryRepository.Delete();
            _playerUnits = null;
        }

        private InventoryUnit GetOrCreateUnit(string unitId)
        {
            if (!Inventory.ContainsUnit(unitId))
            {
                InitUnitState(unitId);
            }

            return Inventory.GetUnit(unitId);
        }

        public void AddUnitFragments(string unitId, int count)
        {
            var inventoryUnit = GetOrCreateUnit(unitId);
            TryReportGetFragmentsToUnlock(inventoryUnit, count);
            inventoryUnit.Fragments += count;
            _inventoryRepository.Set(Inventory);
            _analytics.ReportResourceGained(CurrencyType.CharacterFragments, count);
            _messenger.Publish(new InventoryChangedMessage());
            
        }

        private void TryReportGetFragmentsToUnlock(InventoryUnit inventoryUnit, int receivedFragments)
        {
            if (inventoryUnit.IsUnlocked) {
                return;
            }
            var fragmentsToUnlock = GetFragmentsToUnlock(inventoryUnit.UnitId);
            if (inventoryUnit.Fragments >= fragmentsToUnlock) {
                return;
            }
            if (inventoryUnit.Fragments + receivedFragments >= fragmentsToUnlock) {
                _analytics.ReportGetFragmentsToUnlock(inventoryUnit.UnitId, _playerProgressService.Progress);
            }
        }

        public int GetFragmentsToUnlock(string unitId)
        {
            var startingStars = _unitCollectionConfig.GetUnitStartingStars(unitId);
            return _unitUnlockConfig.GetPrice(startingStars);
        }        
        
        public bool TryUnlockUnit(string unitId)
        {
            var inventory = Inventory;
            var unit = GetUnit(unitId);
            var fragmentsToUnlock = GetFragmentsToUnlock(unitId);
            if (unit.InventoryUnit.IsUnlocked || fragmentsToUnlock > unit.InventoryUnit.Fragments) return false;
            unit.InventoryUnit.Fragments -= fragmentsToUnlock;
            unit.InventoryUnit.Star = unit.BaseUnit.RankConfig.StartingStars;
            _inventoryRepository.Set(inventory);
            
            _analytics.ReportResourceGained(CurrencyType.Character, 1);
            _analytics.ReportUnitEvent(unitId, UnitEvents.UNLOCKED, new Dictionary<string, object>() {{"progress", _playerProgressService.Progress.TotalProgress}});
            _playerProgress.IncreaseCollectiblesCount(PlayerCollectibles.Character);
            _analytics.ReportUnlockUnit(unitId, _playerProgressService.Progress);
            _messenger.Publish(new InventoryChangedMessage());
            return true;
        }

        public UnitModel FindUnit(string unitId)
        {
            return PlayerUnits.FirstOrDefault(it => it.UnitId == unitId);
        }
        public UnitModel GetUnit(string unitId)
        {
            return FindUnit(unitId) ?? throw new NullReferenceException($"Unit not found in inventory, unitId:= {unitId}");
        }
        public bool ContainsUnit(string unitId)
        {
            return FindUnit(unitId) != null;
        }  
        private List<UnitModel> BuildUnits()
        {
            var units = new List<UnitModel>();
            if (ExistInventory) {
                units = Inventory.Units.Select(it => _unitModelBuilder.BuildUnit(it.UnitId, it, null)).ToList();
            }
            return units;
        }
        
        public List<UnitModel> PlayerUnits => _playerUnits ??= BuildUnits();
        public bool ExistInventory => _inventoryRepository.Exists();
        private Model.Inventory Inventory => _inventoryRepository.Get() ?? new Model.Inventory();

        public UnitModel FindUnlockableUnit()
        {
            return PlayerUnits
                .FirstOrDefault(it => !it.InventoryUnit.IsUnlocked && 
                                      it.InventoryUnit.Fragments >= _unitUnlockConfig.GetPrice(it.BaseUnit.RankConfig.StartingStars));
        }

        public bool IsUnitUnlocked(string unitId) => FindUnit(unitId)?.InventoryUnit?.IsUnlocked == true;

        public IEnumerable<UnitModel> UnlockedUnits => PlayerUnits.Where(it => it.InventoryUnit.IsUnlocked);
        public IEnumerable<string> UnlockedUnitIds => UnlockedUnits.Select(it => it.UnitId);
    }
}