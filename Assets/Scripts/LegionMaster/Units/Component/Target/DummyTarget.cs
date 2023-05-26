using System;
using LegionMaster.Location.GridArena;
using LegionMaster.NavMap.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Target
{
    public class DummyTarget : MonoBehaviour, ITarget
    {
        [Inject]
        private TargetListProvider _targetListProvider;
        [Inject]
        private NavMapService _navMapService;
        [Inject]
        private IGridPositionProvider _gridPositionProvider;
        
        public UnitType UnitType { get; set; }
        public Action OnTargetInvalid { get; set; }
        public Transform Root => transform;
        public Transform Center => transform;
        public bool IsAlive => true;

        private void OnEnable()
        {
            UnitType = UnitType.AI;
        }

        public string TargetId => name;

        private void Awake()
        {
            _targetListProvider.Add(this);
        }

        private void OnDestroy()
        {
            _targetListProvider.Remove(this);
        }

        private void Update()
        {
            _navMapService.UpdateUnitCell(_gridPositionProvider.GetCellByPos(transform.position), UnitType, TargetId);
        }
    }
}