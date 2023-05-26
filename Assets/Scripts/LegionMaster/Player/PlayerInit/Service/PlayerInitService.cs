using LegionMaster.Config;
using LegionMaster.Core.Config;
using LegionMaster.HyperCasual.Store.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.PlayerInit.Config;
using SuperMaxim.Core.Extensions;
using Zenject;

namespace LegionMaster.Player.PlayerInit.Service
{
    public class PlayerInitService
    {
        private readonly StringKeyedConfigCollection<StartingUnitConfig> _initStringKeyedConfig;
        
        private readonly InventoryService _inventoryService;
        private readonly WalletService _walletService;
        private readonly HyperCasualSettingsConfig _hyperCasualSettingsConfig;
        
        public PlayerInitService(StringKeyedConfigCollection<StartingUnitConfig> initStringKeyedConfig, 
            InventoryService inventoryService, 
            WalletService walletService, 
            [InjectOptional] HyperCasualSettingsConfig hyperCasualSettingsConfig = null)
        {
            _initStringKeyedConfig = initStringKeyedConfig;
            _inventoryService = inventoryService;
            _walletService = walletService;
            _hyperCasualSettingsConfig = hyperCasualSettingsConfig;
            Init();
        }

        public void Init()
        {
            if (_inventoryService.ExistInventory) {
                return;
            }
            AddStartingUnits();

            if (AppConfig.IsHyperCasual)
            {
                AddStartingCoins();
            }
        }
        private void AddStartingUnits()
        {
            _initStringKeyedConfig.ForEach(unit => {
                _inventoryService.LoadUnit(unit.UnitId, unit.Level, unit.Star);
            });
        }
        private void AddStartingCoins()
        {
            _walletService.Set(Currency.Soft, _hyperCasualSettingsConfig.InitialCoinsAmount);
        }
    }
}