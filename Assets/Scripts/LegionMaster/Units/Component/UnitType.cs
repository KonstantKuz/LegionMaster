using System;
using LegionMaster.NavMap.Model;
using UnityEngine;

namespace LegionMaster.Units.Component
{
    public enum UnitType
    {
        AI,
        PLAYER
    }
    
    public static class UnitTypeExtension
    {
        public static Quaternion GetSpawnRotation(this UnitType unitType, Transform spawnTransform)
        {
            return unitType switch {
                    UnitType.AI => spawnTransform.rotation * Quaternion.Euler(0, 180f, 0),
                    UnitType.PLAYER => spawnTransform.rotation,
                    _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };
        }  
        public static CellState ToCellState(this UnitType unitType)
        {
            return unitType switch {
                    UnitType.AI => CellState.BusyEnemy,
                    UnitType.PLAYER => CellState.BusyPlayer,
                    _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };
        }  
        public static CellState ToEnemyCellState(this UnitType unitType)
        {
            return unitType switch {
                    UnitType.AI => CellState.BusyPlayer,
                    UnitType.PLAYER => CellState.BusyEnemy, 
                    _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };
        }
        public static UnitType EnemyUnitType(this UnitType unitType)
        {
            return unitType switch
            {
                UnitType.AI => UnitType.PLAYER,
                UnitType.PLAYER => UnitType.AI,
                _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };
        }
    }
}