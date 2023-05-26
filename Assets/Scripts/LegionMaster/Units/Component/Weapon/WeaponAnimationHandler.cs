using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LegionMaster.Units.Component.Weapon
{
    public class WeaponAnimationHandler : MonoBehaviour
    {
        public Action FireEvent;
        
        [UsedImplicitly]
        public void Fire()
        {
            FireEvent?.Invoke();
        }
    }
}