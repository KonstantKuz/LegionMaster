using UnityEngine;

namespace LegionMaster.UI.Screen.Duel
{
    [RequireComponent(typeof(ScreenSwitcher))]
    public class DuelScreen : BaseScreen
    {
        public const ScreenId DUEL_SCREEN = ScreenId.Duel;
        public override ScreenId ScreenId => DUEL_SCREEN; 
        public override string Url => ScreenName;

    }
}