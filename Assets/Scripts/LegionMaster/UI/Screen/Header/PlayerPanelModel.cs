using System;
using LegionMaster.Player.Progress.Service;
using UniRx;

namespace LegionMaster.UI.Screen.Header
{
    public class PlayerPanelModel
    {
        public readonly IObservable<float> LevelProgress;
        public readonly IObservable<int> Level;

        public PlayerPanelModel(PlayerProgressService playerProgressService)
        {
            LevelProgress = playerProgressService
                .ExpAsProperty.Select(it => 1.0f * it / playerProgressService.CurrentLevelConfig.ExpToNextLevel)
                .AsObservable();
            Level = playerProgressService.LevelAsObservable;
        }
    }
}