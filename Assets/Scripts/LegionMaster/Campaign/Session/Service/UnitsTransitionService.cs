using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Location.Arena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.NavMap.Service;
using LegionMaster.Units;
using LegionMaster.Units.Component;
using LegionMaster.Units.Component.Vfx;
using LegionMaster.Units.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Campaign.Session.Service
{
    public class UnitsTransitionService
    {
        private const int MAX_ANGLE_AROUND_CHEST = 180;
        private const int MIN_PLACING_ANGLE_STEP = 30;
        private const int INITIAL_PLACING_RADIUS = 8;
        private const int RADIUS_INCREASE_STEP = 4;
        
        [Inject] private NavMapService _navMapService;
        [Inject] private LocationArena _locationArena;
        [Inject] private UnitService _unitService;
        [Inject] private UnitFactory _unitFactory;
        
        private Transform RewardChest => _locationArena.RewardChest.transform;
        
        public List<Unit> BuildTransitionUnits()
        {
            var aliveUnits = _unitService.InitialUnits
                .Where(it => it.UnitType == UnitType.PLAYER && it.IsAlive);
            var reSpawnedUnits = _unitService.DeadUnitsPositions
                .Where(it => it.UnitType == UnitType.PLAYER)
                .Select(ReSpawnUnit).ToList();
            return aliveUnits.Concat(reSpawnedUnits).ToList();
        }

        public IEnumerator MoveUnitsToChest(List<Unit> transitionUnits, float unitsSpeed)
        {
            transitionUnits = transitionUnits
                .OrderBy(it => Vector3.Distance(it.transform.position, RewardChest.position)).ToList();

            var placesAroundChest = GetUnitPlacesAroundChest(transitionUnits);
            for (var i = 0; i < transitionUnits.Count; i++)
            {
                var unit = transitionUnits[i];
                var place = placesAroundChest[i];
                unit.UnitStateMachine.SetMoveToPointState(place, unitsSpeed);
            }

            for (var i = 0; i < transitionUnits.Count; i++)
            {
                yield return new WaitForTargetReached(transitionUnits[i], placesAroundChest[i]);
                transitionUnits[i].transform.LookAt(_locationArena.RewardChest.transform);
            }
        }

        public IEnumerator MoveUnitsToExit(List<Unit> transitionUnits, float unitsSpeed)
        {
            transitionUnits = transitionUnits
                .OrderBy(it => Vector3.Distance(it.transform.position, GetExitPositionFor(it))).ToList();

            foreach (var unit in transitionUnits)
            {
                unit.UnitStateMachine.SetMoveToPointState(GetExitPositionFor(unit), unitsSpeed);
            }
            
            var nearestToExitUnit = transitionUnits.First();
            var distanceToExit = Vector3.Distance(nearestToExitUnit.transform.position, GetExitPositionFor(nearestToExitUnit));
            var moveToExitTime = distanceToExit / nearestToExitUnit.UnitModel.MoveSpeed.Value;
            yield return new WaitForSeconds(moveToExitTime / 2);
        }

        private Unit ReSpawnUnit(DeadUnitAtCell deadUnit)
        {
            var unitModel = deadUnit.UnitModel;
            var atCellId = GetNearestEmptyCell(deadUnit.CellId);
            if (atCellId == CellId.InvalidCellId)
            {
                throw new NullReferenceException("There is no empty cell for respawn unit.");
            }
            
            var spawnPosition = _locationArena.CurrentGrid.GetCell(atCellId).transform;
            var reSpawnedUnit = _unitFactory.LoadForBattle(unitModel, UnitType.PLAYER, spawnPosition);
            reSpawnedUnit.GetComponent<VfxPlayer>().PlayVfx(VfxType.Resurrection);
            reSpawnedUnit.IsAlive = true;
            return reSpawnedUnit;
        }

        private CellId GetNearestEmptyCell(CellId toCellId)
        {
            if (_navMapService.IsEmpty(toCellId))
            {
                return toCellId;
            }
            
            var nearestEmptyCell = _navMapService.GetNearestEmptyCell(toCellId);
            return nearestEmptyCell?.CellId ?? CellId.InvalidCellId;
        }

        private Vector3 GetExitPositionFor(Unit unit)
        {
            var exitPosition = unit.transform.position;
            exitPosition.z = _locationArena.ExitFromArena.transform.position.z;
            return exitPosition;
        }

        private List<Vector3> GetUnitPlacesAroundChest(List<Unit> transitionUnits)
        {
            var orderedUnitPlaces = new List<Vector3>();
            var positionsAroundChest = GetPositionsAroundChest(transitionUnits.Count).ToList();
            foreach (var unit in transitionUnits)
            {
                var bestPlace = GetNearestToChestPositionForUnit(positionsAroundChest, unit);
                positionsAroundChest.Remove(bestPlace);
                bestPlace.y = unit.transform.position.y;
                orderedUnitPlaces.Add(bestPlace);
            }

            return orderedUnitPlaces;
        }

        private Vector3 GetNearestToChestPositionForUnit(List<Vector3> positionsAroundChest, Unit unit)
        {
            return positionsAroundChest.OrderBy(it => Vector3.Distance(it, RewardChest.position))
                .ThenBy(it => Vector3.Distance(it, unit.transform.position)).First();
        }
        
        private IEnumerable<Vector3> GetPositionsAroundChest(int unitsCount)
        {
            var angleStep = MAX_ANGLE_AROUND_CHEST / (unitsCount + 1);
            angleStep = Mathf.Max(angleStep, MIN_PLACING_ANGLE_STEP);
            
            var currentAngle = angleStep;
            var currentRadius = INITIAL_PLACING_RADIUS;

            for (var i = 0; i < unitsCount; i++)
            {
                var freePlace = RewardChest.position + 
                                Quaternion.Euler(0, currentAngle, 0) * RewardChest.transform.right * currentRadius;
                currentAngle += angleStep;
                if (currentAngle >= MAX_ANGLE_AROUND_CHEST)
                {
                    currentAngle = angleStep;
                    currentRadius += RADIUS_INCREASE_STEP;
                }
                yield return freePlace;
            }
        }
    }
}