using JetBrains.Annotations;
using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Effect.Effects;
using UnityEngine;

namespace LegionMaster.Units.Effect
{
    public interface IEffectOwner
    {
        GameObject EffectContainer { get; }
        void AddEffect(EffectType effectType, EffectParamsConfig paramsConfig, [CanBeNull] Unit caster = null);
        void AddEffect(EffectType effectType, [CanBeNull] Unit initiator = null);
        void AddEffect(BaseEffect effect);
        void RemoveEffect(EffectType effectType);
        bool HasEffect(EffectType effectType);
    }
}