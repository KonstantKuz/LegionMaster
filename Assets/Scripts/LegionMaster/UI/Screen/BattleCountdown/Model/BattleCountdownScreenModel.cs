using LegionMaster.Core.Mode;

namespace LegionMaster.UI.Screen.BattleCountdown.Model
{
    public class BattleCountdownScreenModel
    {
        private const int COUNTDOWN_START_VALUE = 3; 
        private const string COUNTDOWN_FINISH_TEXT_ID = "CountdownFinishText";
        
        private GameMode _gameMode;
        private CountdownModel _countdownModel;


        public BattleCountdownScreenModel(GameMode gameMode)
        {
            _gameMode = gameMode;
            _countdownModel = new CountdownModel() {
                    CountdownStartValue = COUNTDOWN_START_VALUE,
                    CountdownFinishText = COUNTDOWN_FINISH_TEXT_ID,
            };
        }
        
        public GameMode GameMode => _gameMode;
        public CountdownModel CountdownModel => _countdownModel;
    }
}