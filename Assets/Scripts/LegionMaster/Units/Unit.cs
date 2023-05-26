using System;
using LegionMaster.Location.Model;
using LegionMaster.Modifiers;
using LegionMaster.Units.Component;
using LegionMaster.Units.Component.Ai;
using LegionMaster.Units.Component.Ai.Navigation;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Component.Hud;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Effect;
using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Model;
using LegionMaster.Units.Model.Battle;
using ModestTree;
using SuperMaxim.Core.Extensions;
using UnityEngine;

namespace LegionMaster.Units
{
    public class Unit : MonoBehaviour, IWorldObject
    {
        private IDamageable _damageable;
        private IUpdatableUnitComponent[] _updatables;
        private HudOwner _hudOwner;       
        private IEffectOwner _effectOwner;    
        private UnitStateMachine _unitStateMachine;

        public Action<Unit> OnDeath;
        public UnitType UnitType;
        public bool IsAlive { get; set; }
        public IUnitModel UnitModel { get; private set; }
        public string ObjectId => GetComponent<WorldObject>().ObjectId;
        public GameObject GameObject => gameObject;
        public ObjectType ObjectType => GetComponent<WorldObject>().ObjectType;

        private HudOwner HudOwner => _hudOwner ??= GetComponent<HudOwner>();       
        public IEffectOwner EffectOwner => _effectOwner ??= GetComponent<IEffectOwner>();
        public UnitStateMachine UnitStateMachine => _unitStateMachine ??= GetComponent<UnitStateMachine>();
        
        public void Init(IUnitModel model)
        {
            UnitModel = model;
            _damageable = GetComponentInChildren<IDamageable>();
            _updatables = GetComponentsInChildren<IUpdatableUnitComponent>();
            _damageable.OnDeath += OnDeathUnit;

            foreach (var component in GetComponentsInChildren<IInitializableComponent>()) {
                component.Init(this);
            }
        }
        private void Update()
        {
            if (!IsAlive) {
                return;
            }
            UpdateComponents();
        }
        private void OnDestroy()
        {
            OnDeath = null;
        }
        public void Kill()
        {
            _damageable.OnDeath -= OnDeathUnit;
            IsAlive = false;
            OnDeath?.Invoke(this);
            OnDeath = null;
        }
        
        public void SetDamageReceiveEnabled(bool isEnabled)
        {
            _damageable.DamageEnabled = isEnabled;
        }
        public void SetTeam(UnitType unitType)
        {
            gameObject.layer = GetLayer(unitType);
            UnitType = GetComponent<ITarget>().UnitType = unitType;
            GetComponents<Repaintable>().ForEach(it => it.Repaint(unitType));

            if (UnitModel != null)
            {
                HudOwner.Init(this);
            }
        }

        public void SetAiActive(bool isActive)
        {
            GetComponent<INavigatable>()?.SetActive(isActive);
        }

        public void AddModifier(IModifier modifier)
        {
            if (UnitModel is UnitBattleModel battleModel) {
                battleModel.AddModifier(modifier);
            }
            else {
                throw new Exception("addModifier method is called not in battle mode");
            }
        } 
        public void RemoveModifier(IModifier modifier)
        {
            if (UnitModel is UnitBattleModel battleModel) {
                battleModel.RemoveModifier(modifier);
            }
            else {
                throw new Exception("removeModifier method is called not in battle mode");
            }
        }
        
        public void AddBurningEffect(Unit caster)
        {
            Assert.IsNotNull(EffectOwner, $"IEffectOwner is null on the unit, gameObject:= {gameObject.name}");
            if (!EffectOwner.HasEffect(EffectType.Burning)) {
                EffectOwner.AddEffect(EffectType.Burning, caster);
            }
        }
        public void UpdateHud()
        {
            if (UnitModel == null) {
                throw new Exception("Update Hud error, UnitModel is not uninitialized");
            }
            var initializeHealthBarOwner = GetComponent<IHealthBarOwner>(); 
            initializeHealthBarOwner.Init(this); //call only from the menu
            HudOwner.Init(this);
        }
        
        private void OnDeathUnit()
        {
            Kill();
        }
        private void UpdateComponents()
        {
            for (int i = 0; i < _updatables.Length; i++) {
                _updatables[i].UpdateComponent();
            }
        }
        private static int GetLayer(UnitType unitType)
        {
            return unitType switch
            {
                    UnitType.AI => UnitLayers.EnemyUnit,
                    UnitType.PLAYER => UnitLayers.PlayerUnit,
                    _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };
        }

    }
}