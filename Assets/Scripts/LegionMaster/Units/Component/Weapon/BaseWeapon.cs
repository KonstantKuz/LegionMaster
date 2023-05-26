using System;
using LegionMaster.Units.Component.Target;
using UnityEngine;

namespace LegionMaster.Units.Component.Weapon
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public abstract void Fire(ITarget target, Action<GameObject> hitCallback);
    }
}