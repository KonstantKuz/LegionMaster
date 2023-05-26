using LegionMaster.Analytics;
using LegionMaster.Analytics.Wrapper;
using LegionMaster.Analytics.Wrapper.GameAnalytics;
using LegionMaster.BattlePass.Config;
using LegionMaster.BattlePass.Model;
using LegionMaster.BattlePass.Service;
using LegionMaster.Campaign.Config;
using LegionMaster.Campaign.Session.Service;
using LegionMaster.Campaign.Store;
using LegionMaster.Cheats;
using LegionMaster.Config;
using LegionMaster.Config.Serializers;
using LegionMaster.Core.Config;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Config;
using LegionMaster.Duel.Enemy.Config;
using LegionMaster.Duel.Store;
using LegionMaster.Duel.Store.Config;
using LegionMaster.Enemy.Config;
using LegionMaster.Enemy.Service;
using LegionMaster.Factions.Config;
using LegionMaster.Factions.Service;
using LegionMaster.HyperCasual.Config;
using LegionMaster.HyperCasual.Store;
using LegionMaster.HyperCasual.Store.Config;
using LegionMaster.Localization.Config;
using LegionMaster.Localization.Service;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Location.Camera;
using LegionMaster.Location.GridArena;
using LegionMaster.Location.Session.Service;
using LegionMaster.LootBox.Config;
using LegionMaster.LootBox.Service;
using LegionMaster.Migration;
using LegionMaster.NavMap.Service;
using LegionMaster.Notification;
using LegionMaster.Player.Inventory.Config;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.PlayerInit.Config;
using LegionMaster.Player.PlayerInit.Service;
using LegionMaster.Player.Progress.Config;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Player.Squad.Config;
using LegionMaster.Player.Squad.Model;
using LegionMaster.Player.Squad.Service;
using LegionMaster.ProgressUnit.Config;
using LegionMaster.Purchase.Config;
using LegionMaster.Purchase.Service;
using LegionMaster.Quest.Config;
using LegionMaster.Quest.Model;
using LegionMaster.Quest.Service;
using LegionMaster.Repository;
using LegionMaster.Reward.Config;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Reward.Service;
using LegionMaster.Shop.Config;
using LegionMaster.Shop.Service;
using LegionMaster.Tutorial;
using LegionMaster.Tutorial.Model;
using LegionMaster.UI;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.HUD;
using LegionMaster.UI.Loader;
using LegionMaster.UI.Overlay;
using LegionMaster.UI.Screen;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.Main;
using LegionMaster.UI.Screen.Squad.UnitPlaceChanged;
using LegionMaster.UIMessage.Service;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Config;
using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Effect.Service;
using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using LegionMaster.UpgradeUnit.Config;
using LegionMaster.UpgradeUnit.Service;
using SuperMaxim.Messaging;
using UnityEngine;
using Zenject;

namespace LegionMaster.Core.IoC
{
    public class ZenInjectMonoInstaller : MonoInstaller
    {
        [SerializeField] 
        private string _configOverrideFolder;
        [SerializeField]
        private GameApplication _gameApplication;     
        [SerializeField]
        private ScreenSwitcher _screenSwitcher; 
        [SerializeField]
        private LocationArena _locationArena;
        [SerializeField]
        private LocationObjectFactory _locationObjectFactory;
        [SerializeField] 
        private CameraManager _cameraManager; 
        [SerializeField] 
        private DialogManager _dialogManager;
        [SerializeField] 
        private UIMessagePresenter _uiMessagePresenter;      
        [SerializeField] 
        private UIMessageManager _uiMessageManager;
        [SerializeField] 
        private CheatsManager _cheatsManager;
        [SerializeField] 
        private CheatsActivator _cheatsActivator; 
        [SerializeField] 
        private UIRoot _uiRoot;
        [SerializeField]
        private UnitDebugInfoView _unitDebugInfoView;
        [SerializeField]
        private UnitGroupView _unitGroupView;
        [SerializeField]
        private HudContainer _hudContainer;      
        [SerializeField]
        private TutorialService _tutorialService;   
        [SerializeField]
        private LockOverlay _lockOverlay;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ZenInjectMonoInstaller>().FromInstance(this).AsSingle();
            Container.Bind<LocalRepositoryMigrationService>().FromInstance(new LocalRepositoryMigrationService()).AsSingle();

            Container.Bind<GameApplication>().FromInstance(_gameApplication).AsSingle();
            Container.Bind<IMessenger>().FromInstance(Messenger.Default).AsSingle();
            Container.Bind<CameraManager>().FromInstance(_cameraManager).AsSingle();
            Container.Bind<ABTest.ABTest>().AsSingle();
       
            
            RegisterConfigs(Container, _configOverrideFolder);
            RegisterUI();
            RegisterCheats();
            RegisterLocation();
            RegisterUnitServices();
            RegisterRepositories();
            RegisterAnalytics();
            RegisterRewardServices();
            RegisterPlayerServices();
            RegisterEnemyServices();
            RegisterServices();
            RegisterShop();
            RegisterLootBox();
            RegisterBattlePass();
            RegisterDuel();
            RegisterCampaign();
            RegisterHyperCasual();
            RegisterTutorial();
          
        }

        private void RegisterServices()
        {
            Container.Resolve<PlayerInitService>().Init();
            Container.Bind<UnitPlaceChangedMediator>().AsSingle().NonLazy();
            Container.Bind<NotificationService>().AsSingle();
        }
        private void RegisterTutorial()
        {
            _tutorialService.Init();
            Container.Bind<TutorialService>().FromInstance(_tutorialService).AsSingle();
        }
        private void RegisterCheats()
        {
            Container.Bind<CheatsManager>().FromInstance(_cheatsManager).AsSingle();
            Container.Bind<CheatsActivator>().FromInstance(_cheatsActivator).AsSingle();
        }
        private void RegisterDuel()
        {
            Container.Bind<DuelUnitStoreService>().AsSingle();
        }  
        private void RegisterHyperCasual()
        {
            Container.Bind<HyperCasualStoreService>().AsSingle();
            Container.Bind<HyperCasualPurchasedUnitsRepository>().AsSingle();  
            Container.Bind<HyperCasualUnitConfig>().AsSingle();
        }     
        private void RegisterCampaign()
        {
            Container.Bind<CampaignUnitStoreService>().AsSingle();
            Container.Bind<UnitsTransitionService>().AsSingle();
        }
        private void RegisterLootBox()
        {
            Container.Bind<LootBoxOpeningService>().AsSingle();
        }       
        private void RegisterBattlePass()
        {
            Container.Bind<BattlePassService>().AsSingle(); 
            Container.Bind<BattlePassShowChecker>().AsSingle();
        }  
        private void RegisterShop()
        {
            Container.Bind<ShopService>().AsSingle();
            Container.Bind<ShopCollectablesService>().AsSingle().NonLazy();
            Container.Bind<InAppPurchaseService>().AsSingle();     
            Container.Bind<ShopShowChecker>().AsSingle();
        }
        private void RegisterEnemyServices()
        {
            Container.Bind<EnemySquadService>().AsSingle().NonLazy();
        }
        private void RegisterRewardServices()
        {
            Container.Bind<CommonRewardService>().AsSingle().NonLazy();
            Container.Bind<RewardBattleResultService>().AsSingle();
            Container.Bind<IRewardApplyService>().To<RewardApplyService>().AsSingle();
        }

        private void RegisterPlayerServices()
        {
            Container.Bind<PlayerProgressService>().AsSingle().NonLazy();
            Container.Bind<InventoryService>().AsSingle();       
            Container.Bind<WalletService>().AsSingle();
            Container.Bind<PlayerInitService>().AsSingle();
            Container.Bind<PlayerSquadService>().AsSingle();
            Container.Bind<QuestService>().AsSingle();
            Container.Bind<UpdateQuestStateAtMidnight>().AsSingle().NonLazy();
            Container.Bind<QuestMessageHandler>().AsSingle().NonLazy();
            Container.Bind<UnitUpgradableStateProvider>().AsSingle();
        }

        private void RegisterAnalytics()
        {
            Container.Bind<Analytics.Analytics>()
                     .FromNew()
                     .AsSingle()
                     .WithArguments(new IAnalyticsImpl[]
                     {
                             new AppMetricaAnalyticsWrapper(),
                             new GameAnalyticsWrapper(),
                             new GoogleAnalyticsWrapper(),
                             //TODO: not a good decision - will cause bugs in FacebookAnalyticsWrapper that are only reproduced on android/ios
                             //and not in Editor
#if !UNITY_EDITOR && !PLATFORM_STANDALONE
                            new FacebookAnalyticsWrapper()
#endif
                     }).NonLazy();
        }

        private void RegisterRepositories()
        {
            Container.Bind<ISingleModelRepository<SquadModel>>().WithId(GameMode.Battle).To<PlayerSquadRepository>().AsSingle(); 
            Container.Bind<ISingleModelRepository<SquadModel>>().WithId(GameMode.Duel).To<PlayerDuelSquadRepository>().AsSingle();         
            Container.Bind<ISingleModelRepository<SquadModel>>().WithId(GameMode.Campaign).To<PlayerCampaignSquadRepository>().AsSingle();
            Container.Bind<ISingleModelRepository<SquadModel>>().WithId(GameMode.HyperCasual).To<PlayerHyperCasualSquadRepository>().AsSingle();

            Container.Bind<PlayerProgressRepository>().AsSingle();        
            Container.Bind<InventoryRepository>().AsSingle();
            Container.Bind<WalletRepository>().AsSingle();
            Container.Bind<RewardSiteStateRepository>().AsSingle();      
            Container.Bind<BattleSessionRepository>().AsSingle();
            Container.Bind<UnitPlaceChangedRepository>().AsSingle();        
            Container.Bind<LootBoxNotificationRepository>().AsSingle();
            
            Container.Bind<ISingleModelRepository<QuestSectionCollection>>().To<QuestSectionRepository>().AsSingle();
            Container.Bind<ISingleModelRepository<QuestCollection>>().To<QuestRepository>().AsSingle();
            Container.Bind<ISingleModelRepository<QuestRewardList>>().To<ForgottenQuestRewardsRepository>().AsSingle();         
            
            Container.Bind<ISingleModelRepository<BattlePassProgress>>().To<BattlePassProgressRepository>().AsSingle();    
            Container.Bind<ISingleModelRepository<BattlePassRewardCollection>>().To<BattlePassRewardRepository>().AsSingle();
            Container.Bind<BattlePassShownStateRepository>().AsSingle();     
            Container.Bind<TutorialRepository>().AsSingle();    
            Container.Bind<ShopCollectablesRepository>().AsSingle();
        }

        private void RegisterLocation()
        {
            Container.Bind<SessionBuilder>().AsSingle();
            Container.Bind<BattleConfigurationService>().AsSingle();

            var battleSessionService = new BattleSessionServiceWrapper();
            Container.Bind<BattleSessionServiceWrapper>().FromInstance(battleSessionService).AsSingle();
            Container.Bind<IBattleSessionService>().FromInstance(battleSessionService).AsSingle();

            Container.Bind<LocationArena>().FromInstance(_locationArena).AsSingle();
            Container.Bind<IGridPositionProvider>().To<ArenaGridProvider>().AsSingle();
            
            _locationObjectFactory.Init();
            Container.Bind<LocationObjectFactory>().FromInstance(_locationObjectFactory).AsSingle();
        }  
        private void RegisterUnitServices()
        {
            Container.Bind<UnitFactory>().AsSingle();
            Container.Bind<TargetListProvider>().AsSingle();
            Container.Bind<UnitService>().AsSingle();
            Container.Bind<DamageService>().AsSingle();      
            Container.Bind<UnitParameterCalculator>().AsSingle();
            Container.Bind<UpgradeUnitService>().AsSingle();
            Container.Bind<UnitModelBuilder>().AsSingle();
            Container.Bind<NavMapService>().AsSingle();
            Container.Bind<FactionService>().AsSingle();  
            Container.Bind<EffectFactory>().AsSingle();
        }

        private void RegisterUI()
        {
            Container.Bind<LocalizationService>().AsSingle();
            Container.Bind<ScreenSwitcher>().FromInstance(_screenSwitcher).AsSingle();      
            Container.Bind<DialogManager>().FromInstance(_dialogManager).AsSingle();       
            Container.Bind<RewardPresenter>().AsSingle();     
            
            Container.Bind<UIMessagePresenter>().FromInstance(_uiMessagePresenter).AsSingle();      
            Container.Bind<UIMessageManager>().FromInstance(_uiMessageManager).AsSingle();   
            
            Container.Bind<UILoader>().AsSingle();
            Container.Bind<UIRoot>().FromInstance(_uiRoot).AsSingle();
            Container.Bind<UnitDebugInfoView>().FromInstance(_unitDebugInfoView).AsSingle();

            Container.Bind<HudContainer>().FromInstance(_hudContainer);     
            Container.Bind<LockOverlay>().FromInstance(_lockOverlay);
            if (_unitGroupView != null)
            {
                Container.Bind<UnitGroupView>().FromInstance(_unitGroupView).AsSingle();
            }
        }
        
        public static void RegisterConfigs(DiContainer container, string configOverrideFolder = null)
        {
            new ConfigLoader(container, new CsvConfigDeserializer(), configOverrideFolder)
                    .RegisterSingle<AppConfig>(Configs.APP)
                    .RegisterSingle<UnitCollectionConfig>(Configs.UNIT)
                    .RegisterSingle<UpgradeStarsConfigCollection>(Configs.UNIT_UPGRADE_STARS)
                    .RegisterSingle<UnitUnlockConfig>(Configs.UNIT_UNLOCK)
                    .RegisterSingle<EnemiesConfig>(Configs.ENEMIES)
                    .RegisterSingle<RewardBattleCollectionConfig>(Configs.REWARD_BATTLE_RESULT, true)
                    .RegisterSingle<RewardBattleCollectionConfig>(Configs.REWARD_DUEL_RESULT, true)            
                    .RegisterSingle<RewardBattleCollectionConfig>(Configs.REWARD_CAMPAIGN_RESULT, true)
                    .RegisterSingle<RewardSiteCollectionConfig>(Configs.REWARD_SITE)
                    .RegisterSingle<ShopScreenConfig>(Configs.SHOP_SCREEN)
                    .RegisterSingle<LootBoxShopConfig>(Configs.LOOT_BOX_SHOP) 
                    .RegisterSingle<BattlePassConfigList>(Configs.BATTLE_PASS)
                    .RegisterSingle<FactionConfigCollection>(Configs.FACTIONS)
                    .RegisterSingle<LocalizationConfig>(Configs.LOCALIZATION)
                    .RegisterSingle<PlayerSquadConfig>(Configs.CONSTANTS)    
                    .RegisterSingle<MatchEnemiesConfig>(Configs.DUEL_ENEMIES, true)
                    .RegisterSingle<MatchEnemiesConfig>(Configs.CAMPAIGN_ENEMIES, true)
                    .RegisterSingle<DuelConfig>(Configs.CONSTANTS)
                    .RegisterSingle<AdditionalBattleRewardCollectionConfig>(Configs.ADDITIONAL_BATTLE_REWARDS)
                    .RegisterSingle<PackConfigCollection>(Configs.PACK)
                    .RegisterSingle<EffectConfigCollection>(Configs.EFFECTS)       
                    .RegisterSingle<CampaignConfig>(Configs.CAMPAIGN)
                    .RegisterSingle<HyperCasualSettingsConfig>(Configs.HYPERCASUAL_SETTINGS, false, true)
                    .RegisterCollection<QuestSectionRewardId, QuestSectionRewardConfig>(Configs.QUEST_SECTION)
                    .RegisterStringKeyedCollection<LevelConfig>(Configs.PLAYER_LEVELS)
                    .RegisterStringKeyedCollection<ResourceConfig>(Configs.RESOURCE)
                    .RegisterStringKeyedCollection<UpgradeLevelConfig>(Configs.UNIT_UPGRADE_LEVEL)
                    .RegisterStringKeyedCollection<StartingUnitConfig>(Configs.PLAYER_INIT)
                    .RegisterStringKeyedCollection<QuestConfig>(Configs.QUEST)
                    .RegisterStringKeyedCollection<ProductConfig>(Configs.SHOP)
                    .RegisterStringKeyedCollection<DuelProductConfig>(Configs.DUEL_SHOP)           
                    .RegisterStringKeyedCollection<UnitRarityConfig>(Configs.UNITS_RARITY)
                    .RegisterStringKeyedCollection<PurchaseConfig>(Configs.PURCHASE)       
                    .RegisterStringKeyedCollection<MergeableUnitConfig>(Configs.MERGEABLE_MELEE_UNITS, true, true)    
                    .RegisterStringKeyedCollection<MergeableUnitConfig>(Configs.MERGEABLE_RANGED_UNITS, true, true);
               
         
        }
    }
}