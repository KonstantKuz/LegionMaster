using System;
using System.Collections.Generic;
using System.Linq;

namespace LegionMaster.Player.Inventory.Model
{
    public class Inventory
    {
        public List<InventoryUnit> Units;
        public Inventory()
        {
            Units = new List<InventoryUnit>();
        }
        public InventoryUnit FindUnit(string unitId) => Units.FirstOrDefault(it => it.UnitId == unitId);

        public bool ContainsUnit(string unitId)
        {
            return FindUnit(unitId) != null;
        }   
        public void AddUnit(InventoryUnit unit)
        {
            Units.Add(unit);
        }

        public InventoryUnit GetUnit(string unitId) => FindUnit(unitId) ?? throw new NullReferenceException($"Unit is null for unitId:= {unitId}");
    }
}