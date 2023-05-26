using System;
using LegionMaster.Player.Inventory.Model;

namespace LegionMaster.Analytics.Data
{
    public enum CurrencyType
    {
        Character,
        CharacterFragments,
        Chest,
        Soft,
        Hard,
        Exp,
        BattlePassExp,
        BattlePassPremium
    }

    public static class CurrencyExt
    {
        public static CurrencyType ToAnalytics(this Currency currency)
        {
            return currency switch {
                    Currency.Soft => CurrencyType.Soft,
                    Currency.Hard => CurrencyType.Hard,
                    _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, null)
            };
        }
    }
}