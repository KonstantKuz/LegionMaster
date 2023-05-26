using System;
using LegionMaster.UI.Screen.Description;
using LegionMaster.UI.Screen.Description.Model;

namespace LegionMaster.UI.Screen.BattlePass.Model
{
    public class BattlePassPremiumModel
    {
        public bool CanBuyPremium;
        public PriceButtonModel PriceButton;
        public Action BuyPremiumAction;
    }
}