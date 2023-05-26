using System;
using LegionMaster.Duel.Store;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using UniRx;

namespace LegionMaster.UI.Screen.DuelSquad.Model
{ 
    public class DisplayCaseUpdateButtonModel
    {
        public PriceModel Price { get; private set; }
        public Action OnClick { get; private set; }
        public IObservable<bool> Available { get; private set; }
        public BoolReactiveProperty Enabled { get; private set; }

        public static DisplayCaseUpdateButtonModel Create(int price, WalletService walletService, DuelUnitStoreService duelUnitStore, Action onClick)
        {
            var enabledReactiveProperty = new BoolReactiveProperty(true);
            var available = AvailableMoney(price, walletService)
                .CombineLatest(duelUnitStore.IsStockEmptyAsObservable, (availableMoney, isStockEmpty) => availableMoney && !isStockEmpty)
                .CombineLatest(enabledReactiveProperty, (available, enabled) => available && enabled);
            var priceModel = PriceModel.Create(price, available);
            return new DisplayCaseUpdateButtonModel {
                    Price = priceModel,
                    OnClick = onClick,
                    Available = available,
                    Enabled = enabledReactiveProperty
            };
        }

        private static IObservable<bool> AvailableMoney(int price, WalletService walletService) =>
            walletService
                .GetMoneyAsObservable(Currency.DuelToken).Select(it => it >= price);

    }
    
}