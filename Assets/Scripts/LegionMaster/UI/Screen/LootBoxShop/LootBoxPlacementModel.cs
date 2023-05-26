using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class LootBoxPlacementModel
    {
        public string PlacementId;
        public IReadOnlyCollection<IObservable<ProductItemModel>> Products => _products;
        
        private IReadOnlyCollection<ReactiveProperty<ProductItemModel>> _products;
        public LootBoxPlacementModel(string placementId, IEnumerable<ProductItemModel> products)
        {
            PlacementId = placementId;
            _products = products.Select(it => new ReactiveProperty<ProductItemModel>(it)).ToList();
        }
        public void UpdateProducts(IEnumerable<ProductItemModel> products)
        {
            foreach (var productModel in products) {
                var productProperty = _products.First(it => it.Value.ProductId == productModel.ProductId);
                productProperty.SetValueAndForceNotify(productModel);
            }
        }
    }
}