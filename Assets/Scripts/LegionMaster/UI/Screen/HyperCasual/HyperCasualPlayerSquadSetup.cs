using System.Linq;
using LegionMaster.Location.GridArena;
using LegionMaster.UI.Screen.DuelSquad.SquadSetup;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Units;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.HyperCasualMode
{
    public class HyperCasualPlayerSquadSetup : MergeablePlayerSquadSetup
    {
        [Inject]
        private IGridPositionProvider _gridPositionProvider;
        public override GameObject GetUnitObject(PlacedUnitModel unit)
        {
            return UnitsRoot.GetComponentsInChildren<Unit>()
                            .First(it => _gridPositionProvider.GetCellByPos(it.transform.position) == unit.CellId)
                            .GameObject;
        }

        public override GameObject LoadUnit(PlacedUnitModel model)
        {
            var unit = base.LoadUnit(model).GetComponent<Unit>();
            unit.UnitModel.StarBarVisible = unit.UnitModel.HudVisible = false;
            unit.Init(unit.UnitModel);
            return unit.gameObject;
        }
    }
}