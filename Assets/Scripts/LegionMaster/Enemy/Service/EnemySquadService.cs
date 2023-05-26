using System;
using LegionMaster.Config;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Enemy.Config;
using LegionMaster.Enemy.Config;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Player.Progress.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Enemy.Service
{
    public class EnemySquadService
    {
        [Inject(Id = Configs.DUEL_ENEMIES)]
        private MatchEnemiesConfig _duelEnemies; 
        
        [Inject(Id = Configs.CAMPAIGN_ENEMIES)]
        private MatchEnemiesConfig _campaignEnemies;
        
        [Inject] private EnemiesConfig _battleEnemies;
        
        [Inject] private PlayerProgressService _playerProgressService; 

        public EnemySquadConfig GetMatchEnemySquad(GameMode gameMode, int round)
        {
            var match = GetMatch(gameMode);
            return round > match.RoundCount ? match.GetMaxRoundSquad() : match.GetEnemySquad(round);
        }
        public EnemyMatchConfig GetMatch(GameMode gameMode)
        {
            if (!gameMode.IsMatchMode()) {
                throw new Exception($"Game mode:={gameMode} don't belong to the match");
            }
            var battleProgress = gameMode == GameMode.Campaign ? PlayerProgress.BattleCountInfos[gameMode].WinCount : PlayerProgress.BattleCountInfos[gameMode].Count;
            var config = GetMatchConfig(gameMode);
            return battleProgress >= config.MatchCount
                           ? config.GetRandomMatch(battleProgress)
                           : config.GetMatchByIndex(battleProgress);
        }
        public EnemySquadConfig GetEnemySquad(GameMode gameMode)
        {
            return _battleEnemies.GetEnemySquad(GetEnemySquadId(gameMode));
        }
        public int GetEnemySquadId(GameMode gameMode)
        {
            return gameMode switch {
                    GameMode.Battle => _battleEnemies.GetSquadByIndex(Mathf.Min(PlayerProgress.BattleProgress, _battleEnemies.SquadCount - 1)).Id,
                    GameMode.HyperCasual => _battleEnemies.GetSquadByIndex(PlayerProgress.HyperCasualProgress % _battleEnemies.SquadCount).Id,
                    _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
            };
        }
        public bool BattleSquadExists => PlayerProgress.BattleProgress < _battleEnemies.SquadCount;
        private MatchEnemiesConfig GetMatchConfig(GameMode gameMode) => gameMode == GameMode.Duel ? _duelEnemies : _campaignEnemies;
        private PlayerProgress PlayerProgress => _playerProgressService.Progress;
    }
}