
namespace LegionMaster.BattlePass.Model
{
    public enum BattlePassRewardType
    {
        Basic,
        Premium
    }
    
    public static class BattlePassRewardTypeExt
    {
        public static string AnalyticsId(this BattlePassRewardType value)
        {
            return value == BattlePassRewardType.Basic ? "Free" : "Premium";
        }
    }
}