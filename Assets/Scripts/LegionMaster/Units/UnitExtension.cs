using LegionMaster.Units.Config;
using UnityEngine;

namespace LegionMaster.Units
{
    public static class UnitExtension
    {
        public static void SetupCollider(this Unit unit, UnitColliderParams colliderParams)
        {
            var unitCollider = unit.GetComponent<CapsuleCollider>();
            var center = unitCollider.center;
            center.y = colliderParams.CenterHeight;
            unitCollider.center = center;
            unitCollider.radius *= colliderParams.SizeMultiplier;
            unitCollider.height *= colliderParams.SizeMultiplier;
        }
    }
}
