using System;

namespace LegionMaster.Player.Inventory.Model
{
    public enum Currency
    {
        Soft,
        Hard,
        DuelToken,
    }
    public static class CurrencyExt
    {
        public static Currency ValueOf(string name)
        {
            return (Currency) Enum.Parse(typeof(Currency), name, true);
        }
        public static bool ShouldToAnalytics(this Currency currency) => currency == Currency.Soft || currency == Currency.Hard;
    }
}