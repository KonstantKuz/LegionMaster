using LegionMaster.Location.GridArena.Model;
using LegionMaster.Units.Component;
using LegionMaster.Units.Model;

namespace LegionMaster.Units.Service
{
    public struct DeadUnitAtCell
    {
        public UnitType UnitType;
        public IUnitModel UnitModel;
        public CellId CellId;
    }
}