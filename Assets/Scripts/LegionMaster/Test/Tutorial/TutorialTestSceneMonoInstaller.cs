using LegionMaster.Analytics;
using LegionMaster.Core.IoC;
using LegionMaster.Localization.Service;
using LegionMaster.Repository;
using LegionMaster.Tutorial;
using LegionMaster.UI.Screen;
using UnityEngine;
using Zenject;

namespace LegionMaster.Test.Tutorial
{
    public class TutorialTestSceneMonoInstaller : MonoInstaller
    {
        [SerializeField]
        private TutorialService _tutorialService;
        [SerializeField]
        private ScreenSwitcher _screenSwitcher; 
        public override void InstallBindings()
        {
            Container.Bind<LocalizationService>().AsSingle();
            Container.Bind<TutorialService>().FromInstance(_tutorialService).AsSingle();
            Container.Bind<TutorialRepository>().AsSingle();     
            Container.Bind<Analytics.Analytics>()
                     .FromNew()
                     .AsSingle()
                     .WithArguments(new IAnalyticsImpl[]
                     {
                     }).NonLazy();
            Container.Bind<ScreenSwitcher>().FromInstance(_screenSwitcher).AsSingle();      
            ZenInjectMonoInstaller.RegisterConfigs(Container);
        }
    }
}