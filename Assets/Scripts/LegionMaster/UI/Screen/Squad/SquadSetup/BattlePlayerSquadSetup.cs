using System.Linq;
using LegionMaster.Core.Mode;
using LegionMaster.Location.Session.Config;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Squad.Model;
using LegionMaster.Player.Squad.Service;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Units;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Squad.SquadSetup
{
    public class BattlePlayerSquadSetup : PlayerSquadSetup
    {
        public event DragAction OnStartUnitDrag;
        public event DragAction OnFinishUnitDrag;

        [Inject] private InventoryService _inventoryService;
        [Inject] private PlayerSquadService _squadService;
        
        public override GameObject LoadUnit(PlacedUnitModel model) =>
                LoadUnit(model, (_, item) => OnStartUnitDrag?.Invoke(model, item), (_, item) => OnFinishUnitDrag?.Invoke(model, item));
        
        public override GameObject GetUnitObject(PlacedUnitModel unit)
        {
            return UnitsRoot.GetComponentsInChildren<Unit>().First(it => it.ObjectId == unit.Id).GameObject;
        }

        public override void Save()
        {
            _squadService.Set(new SquadModel {
                    Units = PlacedUnits.Where(it => it.IsPlaced)
                                       .Select(it => new UnitSpawnConfig() {
                                               UnitId = it.Id,
                                               CellId = it.CellId,
                                       })
                                       .ToList()
            }, _gameMode);
        }
        public void RemoveUnit(PlacedUnitModel unit)
        {
            var unitObject = GetUnitObject(unit);
            DestroyUnit(unitObject);
            SetCellFullState(unit, false);
        }

        protected override void CreateModelList()
        {
            var squad = _squadService.GetSquad(_gameMode).Value;
            PlacedUnitModelList = _inventoryService.UnlockedUnits.Select(unit => new PlacedUnitModel {
                                                           Id = unit.UnitId,
                                                           CellId = GetCellId(unit.UnitId, squad),
                                                   })
                                                   .ToList();
        }
        protected override void PlaceUnits()
        {
            PlacedUnits.Where(it => it.IsPlaced).ForEach(it => { PlaceUnitInCell(LoadUnit(it), GetGridCellById(it.CellId)); });
        }
    }
}