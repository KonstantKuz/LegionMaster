using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UpgradeUnit.Service;
using LegionMaster.Util;
using UniRx;

namespace LegionMaster.UI.Screen.Army
{
    public class ArmyScreenModel
    {
        private readonly Dictionary<string, ReactiveProperty<UnitButtonModel>> _unitButtons;
        public IEnumerable<IObservable<UnitButtonModel>> UnitButtons =>
            _unitButtons.Values.Select(it => it.ToReadOnlyReactiveProperty());
        

        public ArmyScreenModel(IEnumerable<string> allUnitIds, 
            InventoryService inventoryService,
            UnitUpgradableStateProvider upgradableStateProvider,
            Action<string> onClick)
        {
            _unitButtons = UnitButtonListModel.BuildButtonModelDictionary(
                allUnitIds, 
                onClick,
                IconPath.GetUnitVertical,
                unitId => upgradableStateProvider.Get(unitId).HaveAnyUpgrade);
            DisableNonAvailableUnits(inventoryService);
        }

        private void DisableNonAvailableUnits(InventoryService inventoryService)
        {
            foreach (var pair in _unitButtons)
            {
                pair.Value.Value.Enabled = inventoryService.IsUnitUnlocked(pair.Key);
            }
        }
    }
}