using System;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Component.Target;
using UnityEngine;

namespace LegionMaster.Units.Component.Weapon
{
    public class MeleeWeapon : BaseWeapon
    {
        public override void Fire(ITarget target, Action<GameObject> hitCallback)
        {
            var targetObj = target as MonoBehaviour;
            if (targetObj == null)
            {
                Debug.LogWarning("Target is not a monobehaviour");
                return;
            }

            if (targetObj.gameObject.GetComponent<IDamageable>() == null)
            {
                return;
            }
            hitCallback?.Invoke(targetObj.gameObject);
        }
     
    }
}