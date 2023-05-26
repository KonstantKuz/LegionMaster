using LegionMaster.Core.IoC;
using LegionMaster.Factions.Service;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Arena.Service;
using LegionMaster.Location.GridArena;
using LegionMaster.NavMap.Service;
using LegionMaster.UI.HUD;
using LegionMaster.UI.Loader;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Effect.Service;
using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Test.UnitTestScene
{
    public class UnitTestSceneMonoInstaller : MonoInstaller
    {
        [SerializeField] private LocationObjectFactory _locationObjectFactory;
        [SerializeField] private LocationArena _locationArena;
        [SerializeField] private HudContainer _hudContainer;

        public override void InstallBindings()
        {
            Container.Bind<NavMapService>().AsSingle();
            Container.Bind<LocationObjectFactory>().FromInstance(_locationObjectFactory).AsSingle();
            Container.Bind<LocationArena>().FromInstance(_locationArena).AsSingle();
            Container.Bind<IGridPositionProvider>().FromInstance(_locationArena.CurrentGrid).AsSingle(); //TODO: duplication with main zen inject install. Find a way to hide initialization inside components
            Container.Bind<TargetListProvider>().AsSingle();
            Container.Bind<DamageService>().AsSingle();
            Container.Bind<UnitModelBuilder>().AsSingle();
            Container.Bind<UnitFactory>().AsSingle();
            Container.Bind<UnitService>().AsSingle();
            Container.Bind<FactionService>().AsSingle();
            Container.Bind<HudContainer>().FromInstance(_hudContainer).AsSingle();
            Container.Bind<UILoader>().AsSingle();
            Container.Bind<EffectFactory>().AsSingle();
            ZenInjectMonoInstaller.RegisterConfigs(Container);
            
            _locationObjectFactory.Init();
            Container.Resolve<NavMapService>().InitMap();
        }
    }
}