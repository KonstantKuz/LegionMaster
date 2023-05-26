using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.Session.Config;
using LegionMaster.Player.Squad.Model;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UI.Screen.Squad.SquadSetup;
using LegionMaster.Units;
using LegionMaster.Units.Model;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.DuelSquad.SquadSetup
{
    public class MergeablePlayerSquadSetup : PlayerSquadSetup
    {
        public event DragAction OnStartUnitDrag;
        public event DragAction OnFinishUnitDrag;
        
        [Inject] private UnitModelBuilder _unitModelBuilder;

        public override GameObject LoadUnit(PlacedUnitModel model)
        {
            var unit = LoadUnit(model, (_, item) => OnStartUnitDrag?.Invoke(model, item), 
                                (_, item) => OnFinishUnitDrag?.Invoke(model, item)).GetComponent<Unit>();
            var unitModel = _unitModelBuilder.BuildUnit(model.Id, new UnitUpgradeParams() {
                    Level = UnitUpgradeParams.DEFAULT_LEVEL,
                    Star = model.Star,
            }, null);
            unitModel.StarBarVisible = unitModel.HudVisible = true;
            unit.Init(unitModel);
            return unit.gameObject;
        }
        public override void Save()
        {
            var unitSpawnConfigs = PlacedUnitModelList.Where(it => it.IsPlaced)
                .Select(it => new UnitSpawnConfig()
                {
                    UnitId = it.Id,
                    CellId = it.CellId,
                    UpgradeParams = new UnitUpgradeParams()
                    {
                        Level = UnitUpgradeParams.DEFAULT_LEVEL,
                        Star = it.Star,
                    }
                }).ToList();
            
            SquadService.Set(new SquadModel {Units = unitSpawnConfigs}, _gameMode);
        }
        public override GameObject GetUnitObject(PlacedUnitModel unit)
        {
            return UnitsRoot.GetComponentsInChildren<Unit>()
                            .First(it => it.ObjectId == unit.Id && it.UnitModel.Star == unit.Star)
                            .GameObject;
        }
        public PlacedUnitModel FindMergeableUnit(PlacedUnitModel unitModel) => 
                PlacedUnitModel.GetMergeableUnits(PlacedUnits, unitModel.Id, unitModel.Star).FirstOrDefault(it => it != unitModel);
        
        public List<Unit> FindUnitObjects(string unitId, int star)
        {
            return UnitsRoot.GetComponentsInChildren<Unit>().Where(it => it.ObjectId == unitId && it.UnitModel.Star == star).ToList();
        }
        public PlacedUnitModel FindPlacedUnit(string unitId, int star)
        {
            return PlacedUnitModelList.FirstOrDefault(it => it.Id == unitId && it.Star == star);
        }
        public PlacedUnitModel GetPlacedUnit(string unitId, int star)
        {
            return FindPlacedUnit(unitId, star) ?? throw new NullReferenceException($"PlacedUnitModel not found by unitId:= {unitId}");
        }
        protected override void CreateModelList()
        {
            var squad = SquadService.GetSquad(_gameMode);
            var units = squad.Value.Units;
            PlacedUnitModelList = units.Select(unit => new PlacedUnitModel {
                                                      Id = unit.UnitId, 
                                                      CellId = unit.CellId, 
                                                      Star = unit.UpgradeParams?.Star ?? 0,
                                              }).ToList();
        }
        protected override void PlaceUnits()
        {
            PlacedUnitModelList.Where(it => it.IsPlaced)
                               .ForEach(it => { PlaceUnitInCell(LoadUnit(it), GetGridCellById(it.CellId)); });
        }
    }
}