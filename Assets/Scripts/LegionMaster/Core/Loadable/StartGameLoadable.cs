using JetBrains.Annotations;
using LegionMaster.UI.Screen;
using Zenject;

namespace LegionMaster.Core.Loadable
{
    [PublicAPI]
    public class StartGameLoadable : AppLoadable
    {
        [Inject]
        private ScreenSwitcher _screenSwitcher;

        private readonly string _screenUrl;

        public StartGameLoadable(string screenUrl)
        {
            _screenUrl = screenUrl;
        }

        protected override void Run()
        {
            _screenSwitcher.SwitchTo(_screenUrl);
            Next();
        }
    }
}