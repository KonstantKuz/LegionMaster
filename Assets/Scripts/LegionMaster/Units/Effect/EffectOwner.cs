using System.Collections.Generic;
using System.Linq;
using LegionMaster.Units.Component;
using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Effect.Effects;
using LegionMaster.Units.Effect.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Effect
{
    public class EffectOwner : MonoBehaviour, IInitializableComponent, IEffectOwner, IUpdatableUnitComponent
    {
        private readonly Dictionary<EffectType, Queue<BaseEffect>> _effects = new Dictionary<EffectType, Queue<BaseEffect>>();

        [Inject]
        private EffectFactory _effectFactory;
        
        private Unit _owner;
        
        public GameObject EffectContainer => gameObject;

        public void Init(Unit unit)
        {
            _owner = unit;
        }

        public void AddEffect(EffectType effectType, EffectParamsConfig paramsConfig, Unit caster = null)
        {
            if (!_owner.IsAlive) {
                return;
            }
            var effect = _effectFactory.Create(effectType, this, paramsConfig, caster);
            AddEffect(effect);
        }

        public void AddEffect(EffectType effectType, Unit caster = null)
        {
            if (!_owner.IsAlive) {
                return;
            }
            var effect = _effectFactory.Create(effectType, this, caster);
            AddEffect(effect);
        }

        public void AddEffect(BaseEffect effect)
        {
            if (!_owner.IsAlive) {
                return;
            }
            if (!_effects.ContainsKey(effect.EffectType)) {
                CreateQueueAndRun(effect);
            } else {
                AddToQueue(effect);
            }
        }

        public void RemoveEffect(EffectType effectType)
        {
            if (!_effects.ContainsKey(effectType)) {
                Debug.LogWarning($"Effect already removed, effectType:= {effectType}");
                return;
            }
            StopEffect(effectType);
            if (_effects[effectType].Count != 0) {
                RunNextEffect(effectType);
            } else {
                _effects.Remove(effectType);
            }
        }

        public bool HasEffect(EffectType effectType) => _effects.ContainsKey(effectType);
       
        public void UpdateComponent()
        {
            _effects.Values.ToList().ForEach(it => { it.Peek().OnTick(); });
        }

        private void CreateQueueAndRun(BaseEffect effect)
        {
            var queue = new Queue<BaseEffect>();
            queue.Enqueue(effect);
            _effects[effect.EffectType] = queue;
            effect.Run();
        }

        private void AddToQueue(BaseEffect effect)
        {
            var queue = _effects[effect.EffectType];
            queue.Enqueue(effect);
        }

        private void StopEffect(EffectType effectType)
        {
            var queue = _effects[effectType];
            var activeEffect = queue.Dequeue();
            activeEffect.Stop();
            Destroy(activeEffect);
        }

        private void RunNextEffect(EffectType effectType)
        {
            var queue = _effects[effectType];
            var nextEffect = queue.Peek();
            nextEffect.Run();
        }

   
    }
}