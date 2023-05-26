namespace LegionMaster.UpgradeUnit.Model
{
    public struct UnitUpgradableState
    {
        public bool IsUnlocked;
        public bool CanCraft;
        public bool CanUpgradeLevel;
        public int CraftPrice;
        public int LevelUpgradePrice;

        public bool HaveAnyUpgrade => CanCraft || CanUpgradeLevel;
    }
}