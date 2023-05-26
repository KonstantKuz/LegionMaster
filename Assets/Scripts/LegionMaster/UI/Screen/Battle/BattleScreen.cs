using LegionMaster.Location.Camera;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Battle
{
    [RequireComponent(typeof(ScreenSwitcher))]
    public class BattleScreen : BaseScreen
    {
        public const ScreenId BATTLE_SCREEN = ScreenId.Battle;
        public override ScreenId ScreenId => BATTLE_SCREEN;
        public override string Url => ScreenName;
        
        [Inject] private CameraManager _cameraManager;
        [SerializeField] private float _cameraSwitchTime = 0.5f;

        private void OnEnable()
        {
            _cameraManager.SwitchTo(CameraManager.BATTLE_PLAY_MODE, _cameraSwitchTime);
        }
    }
}