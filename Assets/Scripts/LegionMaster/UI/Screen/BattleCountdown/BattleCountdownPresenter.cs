using System.Collections;
using JetBrains.Annotations;
using LegionMaster.Core.Mode;
using LegionMaster.Location.Arena;
using LegionMaster.UI.Screen.Battle;
using LegionMaster.UI.Screen.BattleCountdown.Model;
using LegionMaster.UI.Screen.BattleCountdown.View;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.BattleCountdown
{
    public class BattleCountdownPresenter : BaseScreen
    {
        private const ScreenId BATTLE_COUNTDOWN_SCREEN = ScreenId.BattleCountdown;
        public static readonly string URL = BattleScreen.BATTLE_SCREEN + "/" + BATTLE_COUNTDOWN_SCREEN;
        public override ScreenId ScreenId => BATTLE_COUNTDOWN_SCREEN;
        public override string Url => URL;

        [SerializeField]
        private BattleCountdownView _view;
        [Inject]
        private ScreenSwitcher _screenSwitcher;
        [Inject]
        private LocationArena _locationArena;

        private BattleCountdownScreenModel _model;
        
        
        [PublicAPI]
        public void Init(GameMode mode)
        {
            _model = new BattleCountdownScreenModel(mode);
            _view.Init(_model.CountdownModel, StartBattle);
          
        }
        private void StartBattle() => _screenSwitcher.SwitchTo(BattlePresenter.URL, true, _model.GameMode);

        private void OnDisable()
        {
            _model = null;
        }
        public override IEnumerator Hide()
        {
            _locationArena.HideSquadSetups();
            yield return base.Hide();
        }
    }
}