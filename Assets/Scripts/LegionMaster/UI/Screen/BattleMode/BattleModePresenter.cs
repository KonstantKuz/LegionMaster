using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Camera;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.BattleMode.Model;
using LegionMaster.UI.Screen.BattleMode.View;
using LegionMaster.UI.Screen.Menu;
using LegionMaster.UI.Screen.Squad;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.BattleMode
{
    public class BattleModePresenter : BaseScreen
    {
        public const ScreenId BATTLE_MODE_SCREEN = ScreenId.BattleMode;
        public static readonly string URL = MenuScreen.MENU_SCREEN + "/" + BATTLE_MODE_SCREEN;
        public override ScreenId ScreenId => BATTLE_MODE_SCREEN;
        public override string Url => URL;

        [SerializeField] private ActionButton _toBattleButton;
        [SerializeField] private float _cameraSwitchTime = 0.5f;
        [SerializeField] private EnemyLevelView _enemyLevelView;
        
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private LocationArena _locationArena;
        [Inject] private CameraManager _cameraManager;
        [Inject] private EnemySquadService _enemySquad;

        private BattleModeScreenModel _model;
        private void OnEnable()
        {
            _locationArena.CurrentMode = GameMode.Battle;
            _locationArena.ShowSquadSetups();
            _cameraManager.SwitchTo(CameraManager.BATTLE_MODE, _cameraSwitchTime);
  
            
            _model = new BattleModeScreenModel(_enemySquad);
            _enemyLevelView.Init(_model.EnemyLevel);
            _toBattleButton.Init(StartBattle);
            _toBattleButton.gameObject.SetActive(_model.CanStartBattle);
        }
        private void StartBattle()
        {
            _screenSwitcher.SwitchTo(SquadPresenter.URL);
        }
        private void OnDisable()
        {
            if (_locationArena)
            {
                _locationArena.HideSquadSetups();
            }
            _model = null;
        }
    }
}