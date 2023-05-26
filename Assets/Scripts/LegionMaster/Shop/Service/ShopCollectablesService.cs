using LegionMaster.Player.Progress.Service;
using LegionMaster.Repository;
using LegionMaster.Shop.Data;
using LegionMaster.Shop.Message;
using SuperMaxim.Messaging;

namespace LegionMaster.Shop.Service
{
    public class ShopCollectablesService
    {
        private readonly Analytics.Analytics _analytics;   
        private readonly ShopCollectablesRepository _repository;    
        private readonly PlayerProgressService _playerProgress;
        
        public ShopCollectablesService(IMessenger messenger, Analytics.Analytics analytics, ShopCollectablesRepository repository, PlayerProgressService playerProgress)
        {
            _analytics = analytics;
            _repository = repository;
            _playerProgress = playerProgress;
            messenger.Subscribe<ProductBuyingMessage>(OnProductBuying);
        }
        public bool HasBoughtProduct(string productId) => ProductCollectables.GetPurchaseCount(productId) > 0;
        private void OnProductBuying(ProductBuyingMessage evn)
        {
            var productId = evn.ProductId;
            var productCollectables = ProductCollectables;
            productCollectables.Increase(productId);
            _repository.Set(productCollectables);
            _analytics.ReportProductPurchase(productId, ProductCollectables.GetPurchaseCount(productId), ProductCollectables.TotalPurchaseNumber, _playerProgress.Progress.TotalProgress);
        }
        private ProductCollectables ProductCollectables => _repository.Get() ?? new ProductCollectables();
    }
}