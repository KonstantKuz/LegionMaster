using System;
using System.Linq;
using LegionMaster.HyperCasual.Config;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.UI.Screen.Squad.SquadSetup;
using LegionMaster.Units.Component.Vfx;
using UnityEngine;

namespace LegionMaster.UI.Screen.HyperCasualMode
{
    public class HyperCasualUnitMerger : MonoBehaviour
    {
        private HyperCasualPlayerSquadSetup _squadSetup;
        private UnitCursor _unitCursor;
        private UnitPlacer _unitPlacer;
        private HyperCasualUnitConfig _hyperCasualUnitConfig;
        public void Init(HyperCasualPlayerSquadSetup squadSetup, UnitCursor unitCursor, UnitPlacer unitPlacer, HyperCasualUnitConfig hyperCasualUnitConfig)
        {
            _squadSetup = squadSetup;
            _unitCursor = unitCursor;
            _unitPlacer = unitPlacer;
            _hyperCasualUnitConfig = hyperCasualUnitConfig;
        }

        public bool CanMerge(PlacedUnitModel selectedUnit, PlacedUnitModel targetUnit)
        {
            return _hyperCasualUnitConfig.GetMergeLevelOf(selectedUnit.Id) < _hyperCasualUnitConfig.GetMaxMergeLevelFor(selectedUnit.Id) && 
                   selectedUnit.CanMergeWith(targetUnit.Id, targetUnit.Star);
        }
        
        public void Merge(PlacedUnitModel selectedUnit, PlacedUnitModel targetUnit)
        {
            RemoveFromSquad(selectedUnit, _unitCursor.AttachedUnit); 
            RemoveFromSquad(targetUnit, _squadSetup.GetUnitObject(targetUnit));
            
            var nextUnitModel = GetNextLevelUnitModel(targetUnit);
            _squadSetup.AddToModelList(nextUnitModel);
            var unitObj = _unitPlacer.PlaceInCellNewUnit(nextUnitModel, _unitCursor.SelectedCell);
            
            var vfxPlayer = unitObj.GetComponent<VfxPlayer>();
            vfxPlayer.PlayVfx(VfxType.MergeStars);
            _unitCursor.Detach();
        }
        private void RemoveFromSquad(PlacedUnitModel unitModel, GameObject unitObject)
        {
            _squadSetup.RemoveFromModelList(unitModel);
            _squadSetup.DestroyUnit(unitObject);
        }

        private PlacedUnitModel GetNextLevelUnitModel(PlacedUnitModel fromUnit)
        {
            return new PlacedUnitModel()
            {
                Id = GetNextUnitId(fromUnit.Id),
                CellId = fromUnit.CellId,
            };
        }

        private string GetNextUnitId(string fromUnit)
        {
            var nextUnit = _hyperCasualUnitConfig.UnitsGroupFor(fromUnit)
                           .SkipWhile(it => it.UnitId != fromUnit)
                           .Skip(1).FirstOrDefault();
            
            return nextUnit != null ? nextUnit.UnitId : 
                throw new NullReferenceException($"There is no available unit better than {fromUnit}. Check merge possibility before merge.");
        }
    }
}