using EasyButtons;
using LegionMaster.UI.Screen;
using UnityEngine;
using Zenject;

namespace LegionMaster.Test.Tutorial
{
    public class TutorialTestSetup : MonoBehaviour
    {
        [Inject] private ScreenSwitcher _screenSwitcher;

        [Button]
        private void SwitchTo(string screenUrl)
        {
            _screenSwitcher.SwitchTo(screenUrl);
        }
    }
}