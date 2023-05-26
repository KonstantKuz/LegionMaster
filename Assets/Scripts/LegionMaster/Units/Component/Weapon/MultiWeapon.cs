using System;
using LegionMaster.Units.Component.Target;
using UnityEngine;

namespace LegionMaster.Units.Component.Weapon
{
    public class MultiWeapon: BaseWeapon
    {
        [SerializeField] private BaseWeapon[] _weapons;
        [SerializeField] private bool _singleCallbackCall;
        public override void Fire(ITarget target, Action<GameObject> hitCallback)
        {
            for (int i = 0; i < _weapons.Length; i++)
            {
                _weapons[i].Fire(target, !_singleCallbackCall || i == 0 ? hitCallback : obj => { });
            }
        }
    }
}