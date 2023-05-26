using System.Collections.Generic;

namespace LegionMaster.Units.Component.Hud
{
    public enum UnitHudOwnerType
    {
        Player,
        Enemy,
    }

    public static class UnitHudOwnerTypeExtension
    {
        private static readonly Dictionary<UnitHudOwnerType, string> PREFAB_PATHS = new Dictionary<UnitHudOwnerType, string>() {
                [UnitHudOwnerType.Player] = "Content/UI/HealthBar/PlayerHealthBar",
                [UnitHudOwnerType.Enemy] = "Content/UI/HealthBar/EnemyHealthBar",
        };
        
        public static string GetPrefabPath(this UnitHudOwnerType type)
        {
            return PREFAB_PATHS[type];
        }
    }
}