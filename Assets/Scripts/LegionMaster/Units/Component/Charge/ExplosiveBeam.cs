using LegionMaster.Location.GridArena;
using LegionMaster.NavMap.Service;
using LegionMaster.Units.Component.Target;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Charge
{
    public class ExplosiveBeam : Beam
    {
        [SerializeField]
        private int _damageRadius = 1;

        [Inject]
        private NavMapService _navMapService;
        [Inject]
        private IGridPositionProvider _gridPositionProvider;
        [Inject]
        private TargetListProvider _targetListProvider;

        protected override void Hit(ITarget target)
        {
            var targetCellId = _gridPositionProvider.GetCellByPos(target.Root.position);
            var enemies =
                    _navMapService.NavMap.GetEnemiesInRange(targetCellId, target.UnitType.ToCellState(), _damageRadius, _targetListProvider);
            enemies.ForEach(base.Hit);
        }
    }
}