using System;
using LegionMaster.Units.Component;
using LegionMaster.Units.Component.Hud;
using UnityEngine;

namespace LegionMaster.UI.Screen.Battle.UnitHud
{
    public class UnitHudModel
    {
        public readonly Transform Position;     
        public Action DeathCallback;
        
        private readonly IUnitHudOwner _owner;

        public UnitHudModel(IUnitHudOwner owner)
        {
            _owner = owner;
            Position = owner.HudPosition;            
            owner.OnRemoveHud += OnRemoveHud;
        }
        
        private void OnRemoveHud()
        {
            DeathCallback?.Invoke(); 
        }
        
        public void Dispose()
        {
            _owner.OnRemoveHud -= OnRemoveHud;
        }        
    }
}