using System;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Effect;
using LegionMaster.Units.Effect.Config;
using UnityEngine;

namespace LegionMaster.Units.Component.Ability
{
    [RequireComponent(typeof(UnitWithEnergy))]
    public class PyromaniacAbility : BaseAbility
    {
        private IEffectOwner _effectOwner;
        protected override void Awake()
        {
            base.Awake();
            _effectOwner = Owner.GetComponent<IEffectOwner>();
        }

        public override void StartAbility(Action endCallback)
        {
            base.StartAbility(endCallback);
            _effectOwner.AddEffect(EffectType.Pyromaniac);
            SignalAbilityFinished();    
        }
        public override void OnTick()
        {
            
        }
        public override void StopAbility()
        {
            
        }

    }
}