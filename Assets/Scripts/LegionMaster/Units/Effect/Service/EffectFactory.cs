using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Effect.Effects;
using Zenject;

namespace LegionMaster.Units.Effect.Service
{
    public class EffectFactory
    {
        private readonly Dictionary<EffectType, Type> _effects = new Dictionary<EffectType, Type>() {
                {EffectType.Stun, typeof(StunEffect)},
                {EffectType.Pyromaniac, typeof(PyromaniacEffect)},
                {EffectType.Burning, typeof(BurningEffect)}
        };
        [Inject]
        private DiContainer _container;
        [Inject]
        private EffectConfigCollection _effectConfigCollection;
        
        public BaseEffect Create(EffectType effectType, IEffectOwner owner, [CanBeNull] Unit caster = null)
        {
            EnsureHasEffect(effectType);
            
            var effect = CreateEffectComponent(effectType, owner);
            return effect.Configure(GetConfig(effectType), owner, caster);
        }

        public BaseEffect Create(EffectType effectType, EffectOwner owner, EffectParamsConfig paramsConfig, Unit caster = null)
        {
            EnsureHasEffect(effectType);
            
            var effect = CreateEffectComponent(effectType, owner);
            var config = GetConfig(effectType);
            config.Params = paramsConfig;
            return effect.Configure(config, owner, caster);
        }

        private EffectConfig GetConfig(EffectType effectType)
        {
            return _effectConfigCollection.Get(effectType);
        }

        private BaseEffect CreateEffectComponent(EffectType effectType, IEffectOwner owner)
        {
            return (BaseEffect) _container.InstantiateComponent(_effects[effectType], owner.EffectContainer);
        }

        private void EnsureHasEffect(EffectType effectType)
        {
            if (!_effects.ContainsKey(effectType)) {
                throw new ArgumentException($"Unsupported effect type {effectType}");
            }
        }
    }
}