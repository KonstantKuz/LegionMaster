using System;
using JetBrains.Annotations;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Units.Config;

namespace LegionMaster.Units.Model.Meta
{
    public class RankedUnitModel
    {
        private readonly RankConfig _rankConfig;

        private RankedUnitModel(RankConfig rankConfig, [NotNull] InventoryUnit inventoryUnit)
        {
            _rankConfig = rankConfig;
            InventoryUnit = inventoryUnit;
        }
        public static RankedUnitModel Create(RankConfig rankConfig, [NotNull] InventoryUnit inventoryUnit)
        {
            return new RankedUnitModel(rankConfig, inventoryUnit);
        }

        public RarityType RarityType => _rankConfig.RarityType;
        
        public int StartingStars => _rankConfig.StartingStars;
        
        public int Star
        {
            get => Math.Max(InventoryUnit.Star, RankConfig.MIN_STAR_VALUE); 
            set => InventoryUnit.Star = Math.Max(RankConfig.MIN_STAR_VALUE, Math.Min(value, RankConfig.MAX_STAR_VALUE));
        } 
        public int Level => InventoryUnit.Level;

        public string[] Fractions => _rankConfig.Fractions;
        public int Set => _rankConfig.Set;
        
        [NotNull] public InventoryUnit InventoryUnit { get; }
    }
}