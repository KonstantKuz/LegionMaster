using System;
using LegionMaster.Duel.Store;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using UniRx;

namespace LegionMaster.UI.Screen.DuelSquad.Model
{
    public class PriceModel
    {
        public string Price { get; private set; }
        public IObservable<bool> Available { get; private set; }

        public static PriceModel Create(int price, IObservable<bool> available)
        {
            return new PriceModel() {
                    Price = price.ToString(),
                    Available = available,
            };
        }
        public static PriceModel CreateForUnit(string unitId, WalletService walletService, DuelUnitStoreService duelUnitStore)
        {
            int price = duelUnitStore.GetUnitPrice(unitId);
            return Create(price, walletService.GetMoneyAsObservable(Currency.DuelToken).Select(it => it >= price).AsObservable());
        }  
      
    }
}