using LegionMaster.Core.IoC;
using LegionMaster.Purchase.Service;
using Zenject;

namespace LegionMaster.Test.Purchase
{
    public class PurchaseTestSceneMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<InAppPurchaseService>().AsSingle();
            ZenInjectMonoInstaller.RegisterConfigs(Container);
        }
    }
}