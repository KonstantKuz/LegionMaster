using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Modifiers;
using LegionMaster.Units.Effect.Config;
using SuperMaxim.Core.Extensions;
using UnityEngine;

namespace LegionMaster.Units.Effect.Effects
{
    public abstract class BaseEffect : MonoBehaviour
    {
        protected EffectConfig _config;
        protected IEffectOwner _effectOwner;
        protected IReadOnlyList<IModifier> _modifiers;
        protected Unit _ownUnit;
        [CanBeNull]
        protected Unit _caster; //in some cases we need the parameters of the unit that added the effect
        
        private float _time;
        private bool _timerStarted;
        protected bool HasModifiers => _modifiers.Count != 0;
        public abstract EffectType EffectType { get; }

        public BaseEffect Configure(EffectConfig config, IEffectOwner effectOwner, [CanBeNull] Unit caster = null)
        {
            _config = config;
            _effectOwner = effectOwner;
            _caster = caster;
            _ownUnit = GetComponent<Unit>();
            _modifiers = config.Modifiers
                               .Select(ModifierFactory.Create)
                               .ToList();
            _ownUnit.OnDeath += OnDeath;
            return this;
        }

        private void OnDeath(Unit unit)
        {
            _ownUnit.OnDeath -= OnDeath;
            _timerStarted = false;
            FinishEffect();
        }

        public virtual void Run()
        {
            if (HasModifiers) {
                _modifiers.ForEach(it => _ownUnit.AddModifier(it));
            }
            if (_config.AutoFinish) {
                StartFinishTimer();
            }
        }

        private void StartFinishTimer()
        {
            _time = 0.0f;
            _timerStarted = true;
        }

        public virtual void OnTick()
        {
            if (_config.AutoFinish) {
                UpdateFinishTimer();
            }
        }

        private void UpdateFinishTimer()
        {
            if (!_timerStarted) {
                return;
            }
            _time += Time.deltaTime;
            if (_time < _config.Params.Time) {
                return;
            }
            _timerStarted = false;
            FinishEffect();
        }

        public void FinishEffect()
        {
            _effectOwner.RemoveEffect(EffectType);
        }

        public virtual void Stop()
        {
            _ownUnit.OnDeath -= OnDeath;
            if (HasModifiers) {
                _modifiers.ForEach(it => _ownUnit.RemoveModifier(it));
            }
        }
    }
}