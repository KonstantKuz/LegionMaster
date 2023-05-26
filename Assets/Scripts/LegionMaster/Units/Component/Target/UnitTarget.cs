using System;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Target
{
    [RequireComponent(typeof(Unit))]
    public class UnitTarget : MonoBehaviour, ITarget, IUpdatableUnitComponent
    {
        [SerializeField]
        private UnitType _unitType;
        [SerializeField] 
        private Transform _centerTarget;
        [Inject]
        private TargetListProvider _targetListProvider;
        
        private Unit _unit;
        private Animator _animator;

        private static int _idCount; 

        public Action OnTargetInvalid { get; set; }
        public bool IsAlive { get; private set; }

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _centerTarget = _animator.GetBoneTransform(HumanBodyBones.Spine) ?? _centerTarget;
            
            _unit = GetComponent<Unit>();
            _unit.OnDeath += OnDeath;
            IsAlive = true;
            TargetId = $"{_unit.ObjectId}#{_idCount++}";
            _targetListProvider.Add(this);
        }

        private void OnDestroy()
        {
            OnTargetInvalid = null;
            _targetListProvider.Remove(this);
        }
        private void OnDeath(Unit obj)
        {
            IsAlive = false;
            _unit.OnDeath -= OnDeath;
            OnTargetInvalid?.Invoke();
            OnTargetInvalid = null;
        }
        
        public void UpdateComponent() { }
        public Transform Root => transform;

        public Transform Center => _centerTarget;

        public UnitType UnitType
        {
            get => _unitType;
            set =>_unitType = value;
        }

        public string TargetId
        {
            get;
            private set;
        }
    }
}