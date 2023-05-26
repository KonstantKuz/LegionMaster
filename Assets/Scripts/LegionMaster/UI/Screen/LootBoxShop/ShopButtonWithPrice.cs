using LegionMaster.UI.Screen.Description.View;
using UnityEngine;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class ShopButtonWithPrice : ButtonWithPrice
    {
        protected override void SetСanBuyState(bool canBuy)
        {
            CurrencyColor = canBuy ? Color.white : FontColors.NotEnoughCurrencyColor;
        }
    }
}