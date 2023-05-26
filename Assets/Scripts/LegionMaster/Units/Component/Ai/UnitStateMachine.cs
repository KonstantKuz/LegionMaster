using LegionMaster.Location.GridArena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.NavMap.Service;
using LegionMaster.Units.Component.Ability;
using LegionMaster.Units.Component.Ai.Navigation;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Component.Death;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Component.Weapon;
using LegionMaster.Units.Model;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Ai
{
    [RequireComponent(typeof(INavigatable))]
    [RequireComponent(typeof(UnitWithHealth))]
    [RequireComponent(typeof(UnitDeathAnimation))]
    [RequireComponent(typeof(ITarget))]
    [RequireComponent(typeof(AttackUnit))]
    public partial class UnitStateMachine : MonoBehaviour, IUpdatableUnitComponent, IInitializableComponent
    {
        //can be move to unit config, if game-designer would like to setup it
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private string _currentStateName;
                
        [Inject] private IGridPositionProvider _gridPositionProvider;
        [Inject] private NavMapService _navMapService;
        [Inject] private TargetListProvider _targetListProvider;
        
        private INavigatable _navigatable;
        private IUnitAttackModel _attackConfig;
        private Animator _animator;
        private WeaponAnimationHandler _weaponAnimationHandler;
        private UnitDeathAnimation _deathAnimation;
        private UnitType _unitType;
        private BaseAbility _ability;
        private Unit _owner;
        
        private ITarget _selfTarget;
        private AttackUnit _attackUnit;
        
        private BaseState _currentState;  
        private CellId CurrentCellId { get; set; }
        public ITarget Target { get; private set; }
        
        private bool IsTargetInvalid => !(Target is { IsAlive: true });
        private int AttackRangeInCells => _attackConfig.AttackRangeInCells;
        private float AttackInterval => _attackConfig.AttackInterval;
        private UnitType EnemyUnitType => _unitType.EnemyUnitType();
        private BaseAbility Ability => _ability;

        private void Awake()
        {
            _navigatable = GetComponent<LineSegmentNavigatable>();
            _animator = GetComponentInChildren<Animator>();
            Assert.IsNotNull(_animator, "Unit prefab is missing Animator component in hierarchy");
            _weaponAnimationHandler = GetComponentInChildren<WeaponAnimationHandler>();
            _attackUnit = GetComponent<AttackUnit>();
            _deathAnimation = GetComponent<UnitDeathAnimation>();
            _selfTarget = GetComponent<ITarget>();
            _ability = GetComponent<BaseAbility>();
        }
        public void UpdateComponent()
        {
            if (_currentState == null) return;
            
            _currentState.OnTick();
        }
        public void Init(Unit unit)
        {
            _owner = unit;
            _attackConfig = unit.UnitModel.UnitAttack;
            _navigatable.Init(unit.UnitModel.MoveSpeed.Select(it => (float) it).ToReactiveProperty(), _rotationSpeed);
            _unitType = unit.UnitType;
            _owner.OnDeath += OnDeath;
            
            if (unit.UnitModel.AiEnabled)
            {
                var currentCellId = _gridPositionProvider.GetCellByPos(transform.position);
                SetCellId(currentCellId);
                SetIdleState();
            }
            else {
                SetDoNothingState();
            }
        }
        public void SetCellId(CellId cellId)
        {
            CurrentCellId = cellId;
            _navMapService.UpdateUnitCell(CurrentCellId, _unitType, _selfTarget.TargetId);
        }
        public void SetDoNothingState()
        {
            SetState(new DoNothingState(this));
        }  
        public void SetIdleState()
        {
            SetState(new IdleState(this));
        }
        public void SetMoveToPointState(Vector3 targetPosition, float speed)
        {
            SetState(new MoveToPointState(this, targetPosition, speed));
        }
        
        private void SetState(BaseState newState)
        {
            _currentState?.OnExitState();
            _currentState = newState;
            _currentStateName = _currentState.GetType().Name;            
            _currentState.OnEnterState();
        }
        
        private void OnDeath(Unit unit)
        {
            _owner.OnDeath -= OnDeath;
            SetState(new DeathState(this));
        }

        private void OnDrawGizmosSelected()
        {
            if (_currentState is MoveToCellState)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_navigatable.Destination, 0.3f);
            }
        }
        private bool AbilityStart()
        {
            if (!CanStartAbility()) {
                return false;
            }
            SetState(new AbilityState(this));
            return true;
        }
        private bool CanStartAbility() => _owner.UnitModel.AbilityEnabled && _ability != null && _ability.ShouldStart();

    }
}