using LegionMaster.Player.Inventory.Model;

namespace LegionMaster.Player.Inventory.Message
{
    public struct CurrencyChangedMessage
    {
        public Currency Currency;
        public int Delta;

        public CurrencyChangedMessage(Currency currency, int delta)
        {
            Currency = currency;
            Delta = delta;
        }
    }
}