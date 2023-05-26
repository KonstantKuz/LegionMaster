// #define DEBUG_ABILITY
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LegionMaster.Location.GridArena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.NavMap.Model;
using LegionMaster.NavMap.Service;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Component.Weapon;
using LegionMaster.Units.Effect;
using LegionMaster.Units.Effect.Config;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Ability
{
    public class ShieldBearerAbility : BaseAbility
    {
        [SerializeField] private float _speed;
        [SerializeField] private GameObject _vfx;

        private ITarget _ownerTarget;
        private BaseWeapon _weapon;
        private ITarget _target;
        private Unit _targetUnit;
        private IEffectOwner _targetEffectOwner;
        private List<CellId> _path;
        private BulldozingPathBuilder _pathBuilder;
        private Coroutine _bulldozing;
        private Tween _ownerMove;
        private Tween _targetMove;
        
        [Inject] private NavMapService _navMapService;
        [Inject] private TargetListProvider _targetListProvider;
        [Inject] private IGridPositionProvider _gridPositionProvider;

        private CellId OwnerCellId => _gridPositionProvider.GetCellByPos(_ownerTarget.Root.position);
        private CellId TargetCellId => _gridPositionProvider.GetCellByPos(_target.Root.position);
        private CellId PreEdgeCellId => _path.TakeWhile(it => it != _path.Last()).Last();
        
        protected override void Awake()
        {
            base.Awake();
            _ownerTarget = Owner.GetComponent<ITarget>();
            _weapon = Owner.GetComponentInChildren<BaseWeapon>();
            _pathBuilder = new BulldozingPathBuilder(_navMapService);
        }
        public override bool ShouldStart()
        {
#if UNITY_EDITOR && DEBUG_ABILITY 
            return Input.GetKey(KeyCode.A) && HasPath(); 
#else 
            return UnitWithEnergy.IsFull && HasPath(); 
#endif
        }

        public override void StartAbility(Action endCallback)
        {
            base.StartAbility(endCallback);
            SetTargetFromPath();
            StartBulldozing();
            SetVfxEnabled(true);
        }

        public override void OnTick()
        {
        }

        public override void StopAbility()
        {
            if (_bulldozing != null)
            {
                ForceStopBulldozing();
                return;
            }
            
            ClearPath();
            UpdateGridPositions();
            SetVfxEnabled(false);
        }

        private void SetVfxEnabled(bool value)
        {
            _vfx.SetActive(value);
        }

        private bool HasPath()
        {
            _path = _pathBuilder.Build(OwnerCellId, Owner.UnitType.EnemyUnitType())?.ToList(); 
            return _path != null;
        }

        private void SetTargetFromPath()
        {
            var targetCellId = _path.Find(it => _navMapService.GetCell(it).State == Owner.UnitType.ToEnemyCellState());
            _target = _targetListProvider.FindById(_navMapService.GetCell(targetCellId).UnitId);
            _targetUnit = _target.Root.GetComponent<Unit>();
            _targetEffectOwner = _targetUnit.GetComponent<EffectOwner>();
        }

        private void StartBulldozing()
        {
            MarkPathCellsAsReserved();
            StunTarget();
            _bulldozing = StartCoroutine(Bulldozing());
        }

        private void MarkPathCellsAsReserved()
        {
            _path.Select(it => _navMapService.GetCell(it))
                .ForEach(cell => cell.Update(CellState.Reserved, _ownerTarget.TargetId));
            
            var preEdgeCell = _navMapService.GetCell(PreEdgeCellId);
            preEdgeCell.Update(Owner.UnitType.ToCellState(), _ownerTarget.TargetId);
        }

        private void StunTarget()
        {
            var edgeCellId = _path.Last();
            var edgeCellPos = _gridPositionProvider.GetCellPos(edgeCellId);
            var timeToReachEdge = (_ownerTarget.Root.position - edgeCellPos).magnitude / _speed;
            var customStunParams = new EffectParamsConfig {Time = timeToReachEdge};
            _targetEffectOwner.AddEffect(EffectType.Stun, customStunParams);
        }

        private IEnumerator Bulldozing()
        {
            yield return MoveToTarget();
            yield return MoveToEdge();
            OnAbilitySuccess();
        }
        private IEnumerator MoveToTarget()
        {
            var pathToTarget = _path.TakeWhile(it => it != TargetCellId);
            var preTargetCell = pathToTarget.Last();
            var collisionPos = _gridPositionProvider.GetCellPos(preTargetCell);
            var distanceToCollisionPos = (_ownerTarget.Root.position - collisionPos).magnitude;

            _ownerTarget.Root.LookAt(_target.Root);
            _ownerMove = _ownerTarget.Root.DOMove(collisionPos, distanceToCollisionPos / _speed).SetEase(Ease.Linear);
            yield return _ownerMove.WaitForCompletion();
        }
        private IEnumerator MoveToEdge()
        {
            var preEdgeCellPos = _gridPositionProvider.GetCellPos(PreEdgeCellId);
            var distanceToPreEdge = (_ownerTarget.Root.position - preEdgeCellPos).magnitude;

            var edgeCellId = _path.Last();
            var edgeCellPos = _gridPositionProvider.GetCellPos(edgeCellId);
            var distanceToEdge = (_target.Root.position - _gridPositionProvider.GetCellPos(edgeCellId)).magnitude;

            _ownerMove = _ownerTarget.Root.DOMove(preEdgeCellPos, distanceToPreEdge / _speed).SetEase(Ease.Linear);
            _targetMove = _target.Root.DOMove(edgeCellPos, distanceToEdge / _speed).SetEase(Ease.Linear); 
            yield return _targetMove.WaitForCompletion();
        }
        private void OnAbilitySuccess()
        {
            ClearBulldozing();
            SignalAbilityFinished();
            AttackUnit.CustomFire(_target, _weapon, true);
        }

        private void ForceStopBulldozing()
        {
            _ownerMove.Kill();
            _targetMove.Kill();
            StopCoroutine(_bulldozing);
            ClearBulldozing();
            SignalAbilityFinished();
        }

        private void ClearBulldozing()
        {
            _bulldozing = null;
            _ownerMove = null;
            _targetMove = null;
        }
        
        private void UpdateGridPositions()
        {
            Owner.UnitStateMachine.SetCellId(OwnerCellId);
            _targetUnit.UnitStateMachine.SetCellId(TargetCellId);
        }

        private void ClearPath()
        {
            _path.Select(it => _navMapService.GetCell(it))
                .Where(cell => cell.State == CellState.Reserved && cell.UnitId == _ownerTarget.TargetId)
                .ForEach(cell => cell.Update(CellState.Empty));
            _path = null;
        }
    }
}