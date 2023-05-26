using System;
using LegionMaster.Units.Component.HealthEnergy;
using UnityEngine;

namespace LegionMaster.Units.Component.Hud
{
    public interface IUnitHudOwner
    {
        public UnitHudOwnerType OwnerType { get; }        
        public Transform HudPosition { get; }
        public event Action OnRemoveHud;
        public IHealthBarOwner HealthBarOwner { get; }
        public IStarBarOwner StarBarOwner { get; }
    }
}