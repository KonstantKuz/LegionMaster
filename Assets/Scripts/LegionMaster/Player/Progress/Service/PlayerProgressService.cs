using System;
using LegionMaster.Analytics.Data;
using LegionMaster.Campaign.Session.Messages;
using LegionMaster.Config;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Session.Messages;
using LegionMaster.Location.Session.Messages;
using LegionMaster.Player.Progress.Config;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Repository;
using SuperMaxim.Messaging;
using UniRx;
using UnityEngine.Assertions;

namespace LegionMaster.Player.Progress.Service
{
    public class PlayerProgressService
    {
        private readonly PlayerProgressRepository _repository;
        private readonly Analytics.Analytics _analytics;
        private readonly StringKeyedConfigCollection<LevelConfig> _levelStringKeyedConfig;

        private IntReactiveProperty _exp; 
        private IntReactiveProperty _level;

        public IReadOnlyReactiveProperty<int> ExpAsProperty => _exp;
        public IObservable<int> LevelAsObservable => _level;

        public LevelConfig CurrentLevelConfig => Progress.CurrentLevelConfig(_levelStringKeyedConfig); 

        public PlayerProgressService(IMessenger messenger, 
                                     PlayerProgressRepository repository, 
                                     Analytics.Analytics analytics, 
                                     StringKeyedConfigCollection<LevelConfig> levelStringKeyedConfig)
        {
            _repository = repository;
            _analytics = analytics;
            _levelStringKeyedConfig = levelStringKeyedConfig;
            _exp = new IntReactiveProperty(Progress.Exp);
            _level = new IntReactiveProperty(Progress.Level);
            messenger.Subscribe<BattleStartMessage>(OnBattleStart);
            messenger.Subscribe<BattleEndMessage>(OnBattleFinished);
            messenger.Subscribe<DuelBattleEndMessage>(OnDuelBattleFinished);    
            messenger.Subscribe<CampaignBattleEndMessage>(OnCampaignBattleFinished);
        }

        private void OnCampaignBattleFinished(CampaignBattleEndMessage msg)
        {
            var progress = Progress;
            _analytics.ReportCampaignEnd(progress.CampaignProgress, msg.IsPlayerWon);
            IncreaseBattleCount(progress, GameMode.Campaign, msg.IsPlayerWon);
        }

        private void OnDuelBattleFinished(DuelBattleEndMessage msg)
        {
            var progress = Progress;
            _analytics.ReportDuelEnd(progress.DuelProgress, msg.IsPlayerWon);
            IncreaseBattleCount(progress, GameMode.Duel, msg.IsPlayerWon);
        }

        public void ResetProgress()
        {
            _repository.Delete();
            _exp = null;
            _level = null;
        }

        public void IncreaseCollectiblesCount(PlayerCollectibles collectible, int count = 1)
        {
            var progress = Progress;
            progress.IncreasePlayerCollectible(collectible, count);
            _analytics.ReportProfileEvent(collectible, progress.Collectibles[collectible], count);
            SetProgress(progress);
        }

        private void OnBattleStart(BattleStartMessage msg)
        {
            switch (msg.GameMode) {
                case GameMode.Battle:
                    _analytics.ReportBattleStart(Progress.BattleProgress, Progress.BattleCountInfos[GameMode.Battle].Count, Progress.TotalProgress);
                    break;
                case GameMode.HyperCasual:
                    _analytics.ReportHyperCasualBattleStart(Progress.HyperCasualProgress);
                    break;
            }
        }

        private void OnBattleFinished(BattleEndMessage msg)
        {
            var progress = Progress;
            switch (msg.GameMode) {
                case GameMode.Battle:
                    _analytics.ReportBattleEnd(Progress.BattleProgress, msg.IsPlayerWon);
                    break;
                case GameMode.HyperCasual:
                    _analytics.ReportHyperCasualBattleEnd(Progress.HyperCasualProgress, msg.IsPlayerWon);
                    break;
            }
            IncreaseBattleCount(progress, msg.GameMode, msg.IsPlayerWon);
            IncreaseCollectiblesCount(PlayerCollectibles.Battle);            
        }
        private void IncreaseBattleCount(PlayerProgress progress, GameMode mode, bool isPlayerWon)
        {
            progress.IncreaseBattleCount(mode);
            if (isPlayerWon) {
                progress.IncreaseBattleWinCount(mode);
            }
            SetProgress(progress);
        }
        public void AddExp(int amount)
        {
            Assert.IsTrue(amount >= 0, "Added amount of Exp should be non-negative");
            var progress = Progress;
            progress.AddExp(amount, _levelStringKeyedConfig);
            SetProgress(progress);
            _analytics.ReportResourceGained(CurrencyType.Exp, amount);
        }

        private void SetProgress(PlayerProgress progress)
        {
            _repository.Set(progress);
            _level.Value = progress.Level;
            _exp.Value = progress.Exp;
        }

        public PlayerProgress Progress => _repository.Get() ?? new PlayerProgress();

        public int GetWinBattleCount(GameMode gameMode) =>
            Progress.BattleCountInfos[gameMode].WinCount;
    }
}