using LegionMaster.Location.Arena;
using LegionMaster.Units.Component.HealthEnergy;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Model;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace LegionMaster.Units.Component.Ai
{
    [RequireComponent(typeof(NavMeshAgent))]    
    [RequireComponent(typeof(ITargetProvider))]
    public sealed class AiUnit : MonoBehaviour, IMoving, IUpdatableUnitComponent, IInitializableComponent
    {
        private readonly int _moveHash = Animator.StringToHash("Move");
        
        [SerializeField]
        private AiBehaviorBase _behavior;
        
        private NavMeshAgent _navMeshAgent;
        private ITargetProvider _targetProvider;
        private IUnitAttackModel _attackConfig;
        private Animator _animator;

        [Inject] 
        private LocationArena _locationArena;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _targetProvider = GetComponent<ITargetProvider>();
            var damagable = GetComponent<IDamageable>();
            if (damagable != null)
            {
                damagable.OnDeath += Disable;
            }

            _animator = GetComponentInChildren<Animator>();
        }

        private void Disable()
        {
            SetMoving(false);
            _navMeshAgent.enabled = false;
        }

        public void Init(Unit unit)
        {
            _navMeshAgent.speed = unit.UnitModel.MoveSpeed.Value;
            SetMoving(false);
            _navMeshAgent.enabled = true;
            _attackConfig = unit.UnitModel.UnitAttack;
        }
        
        public void UpdateComponent()
        {
            Target = _targetProvider.Target;
            if (_behavior != null)
            {
                _behavior.ProcessTimer(this);
            }
        }
        public void LookAtTarget(float rotationSpeed)
        {
            LookAtDirection = (Target.Root.position - transform.position).normalized;
            var lookAt = Quaternion.LookRotation(LookAtDirection, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookAt, Time.deltaTime * rotationSpeed);
        }
        public ITarget Target { get; set; }
        public Vector3 MovementDirection { get; set; }
        public Vector3 LookAtDirection { get; set; }
        public bool IsMoving => !_navMeshAgent.isStopped;
        public float AttackRange => _attackConfig.AttackRangeInCells * _locationArena.CellSize;

        public void SetMoving(bool isMoving)
        {
            if (_navMeshAgent.isStopped && isMoving)
            {
                _animator.SetTrigger(_moveHash);
            }
            _navMeshAgent.isStopped = !isMoving;
        }

        public void SetDestination(Vector3 targetPos)
        {
            _navMeshAgent.SetDestination(targetPos);
        }

        public void SetRotationEnabled(bool isEnabled)
        {
            _navMeshAgent.updateRotation = isEnabled;
        }
    }
}