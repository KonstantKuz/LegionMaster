namespace LegionMaster.Util
{
    public class IconPath
    {
        public const string REGULAR_REWARD_BACKGROUND_ID = "Regular";
        
        public const string UNIT_PATH_PATTERN = "Content/UI/Icons/Units/{0}";
        public const string UNIT_VERTICAL_PATH_PATTERN = "Content/UI/Icons/Units/Vertical/{0}";
        public const string CURRENCY_PATH_PATTERN = "Content/UI/Icons/Currency/{0}";     
        public const string SHOP_PATH_PATTERN = "Content/UI/Icons/Shop/{0}";  
        public const string REWARD_PATH_PATTERN = "Content/UI/Icons/Reward/{0}";         
        public const string REWARD_BACKGROUND_PATH_PATTERN = "Content/UI/Icons/Reward/Background/{0}";   
        public const string SHOP_PRODUCT_PATH_PATTERN = "Content/UI/Icons/Shop/Products/{0}";  
        public const string DROPPING_LOOT_PATH_PATTERN = "Content/UI/Icons/DroppingLoot/{0}";        
        public const string FRACTION_PATH_PATTERN = "Content/UI/Icons/Factions/{0}";

        public static string GetUnit(string unitId) => string.Format(UNIT_PATH_PATTERN, unitId);
        public static string GetUnitVertical(string unitId) => string.Format(UNIT_VERTICAL_PATH_PATTERN, unitId);
        public static string GetCurrency(string currencyId) => string.Format(CURRENCY_PATH_PATTERN, currencyId);      
        public static string GetShopSectionLabel(string sectionId) => string.Format(SHOP_PATH_PATTERN, sectionId);  
        public static string GetShopProduct(string productId) => string.Format(SHOP_PRODUCT_PATH_PATTERN, productId);  
        public static string GetReward(string rewardId) => string.Format(REWARD_PATH_PATTERN, rewardId);       
        public static string GetRewardBackground(string backgroundId) => string.Format(REWARD_BACKGROUND_PATH_PATTERN, backgroundId);  
        public static string GetDroppingLoot(string lootId) => string.Format(DROPPING_LOOT_PATH_PATTERN, lootId);      
        public static string GetFraction(string fraction) => string.Format(FRACTION_PATH_PATTERN, fraction);
    }
}