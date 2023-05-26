using JetBrains.Annotations;
using LegionMaster.Core.Loadable;

namespace LegionMaster.IOSTransparency.Loadable
{
    [PublicAPI]
    public class IosATTLoadable : AppLoadable
    {
        private IATTListener _attListener;
        
        protected override void Run()
        {
            _attListener = GameAnalyticsATTListener.Create();
            _attListener.OnStatusReceived += OnATTStatusReceived;
            _attListener.Init();
        }

        private void OnATTStatusReceived()
        {
            _attListener.OnStatusReceived -= OnATTStatusReceived;
            Next();
        }
    }
}