using System;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Units.Component.Charge;
using LegionMaster.Units.Component.Target;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Weapon
{
    public class BeamWeapon : BaseWeapon
    {
        [SerializeField] 
        private Transform _barrel;
        [SerializeField]
        private Beam _beam;
        [Inject]
        private LocationObjectFactory _objectFactory;
        
        public override void Fire(ITarget target, Action<GameObject> hitCallback)
        {
            var beam = CreateBeam();
            var pos = _barrel.position;
            var rotationToTarget = RangedWeapon.GetShootRotation(pos, target.Center.position);
            
            beam.transform.SetPositionAndRotation(pos, rotationToTarget);
            beam.Launch(target, hitCallback);
        }

        private Beam CreateBeam()
        { 
            return _objectFactory.CreateObject(_beam.gameObject, _barrel.gameObject).GetComponent<Beam>();
        }
    }
}