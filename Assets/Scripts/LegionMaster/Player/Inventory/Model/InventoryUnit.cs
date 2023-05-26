using LegionMaster.Location.Session.Config;
using LegionMaster.Units.Config;
using UnityEngine;

namespace LegionMaster.Player.Inventory.Model
{
    public class InventoryUnit
    {
        private const int STAR_COUNT_UNIT_LOCKED = -1;
        private readonly UnitUpgradeParams _upgradeParams = new UnitUpgradeParams();

        public static InventoryUnit FromConfig(UnitConfig config)
        {
            return new InventoryUnit {
                    UnitId = config.UnitId,
                    Star = STAR_COUNT_UNIT_LOCKED, 
                    Level = UnitUpgradeParams.DEFAULT_LEVEL
            };
        }

        public static InventoryUnit FromUpgradeParams(string unitId, UnitUpgradeParams upgradeParams)
        {
            return new InventoryUnit {
                    UnitId = unitId, 
                    Star = upgradeParams.Star, 
                    Level = upgradeParams.Level
            };
        }
        
        public string UnitId { get; set; }

        public int Star
        {
            get => _upgradeParams.Star;
            set => _upgradeParams.Star = Mathf.Clamp(value, STAR_COUNT_UNIT_LOCKED, RankConfig.MAX_STAR_VALUE);
        }
        public int Level
        {
            get => _upgradeParams.Level;
            set => _upgradeParams.Level = Mathf.Clamp(value, RankConfig.MIN_LEVEL_VALUE, RankConfig.MAX_LEVEL_VALUE);
        }

        public int Fragments { get; set; }

        public bool IsUnlocked => Star >= RankConfig.MIN_STAR_VALUE;
        public bool IsMaxStarReached => Star >= RankConfig.MAX_STAR_VALUE;
    }
}