using UnityEngine;

namespace LegionMaster.Units
{
    public static class UnitLayers
    {
        public static int PlayerUnit => LayerMask.NameToLayer("PlayerUnit");
        public static int EnemyUnit => LayerMask.NameToLayer("EnemyUnit");
    }
}