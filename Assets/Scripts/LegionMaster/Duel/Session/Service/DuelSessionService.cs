using LegionMaster.Core.Mode;
using LegionMaster.Duel.Config;
using LegionMaster.Duel.Session.Messages;
using LegionMaster.Duel.Session.Model;
using LegionMaster.Enemy.Service;
using LegionMaster.Location.Session.Model;
using LegionMaster.Location.Session.Service;
using LegionMaster.NavMap.Service;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Player.Squad.Service;
using LegionMaster.Repository;
using LegionMaster.Units;
using LegionMaster.Units.Component;
using LegionMaster.Units.Service;
using SuperMaxim.Messaging;
using Zenject;

namespace LegionMaster.Duel.Session.Service
{
    public class DuelSessionService : IBattleSessionService
    {
        private readonly BattleSessionRepository _battleSessionRepository;
        private readonly EnemySquadService _enemySquadService;

        [Inject]
        private BattleConfigurationService _battleConfigurationService;
        [Inject]
        private UnitService _unitService;
        [Inject]
        private IMessenger _messenger;
        [Inject]
        private NavMapService _navMapService;
        [Inject]
        private DuelConfig _duelConfig;
        [Inject]
        private Analytics.Analytics _analytics;
        [Inject]
        private PlayerProgressService _playerProgressService;

        public BattleSession BattleSession => _battleSessionRepository.Require();
        public DuelBattleSession DuelBattleSession => (DuelBattleSession) BattleSession;
        public int Round => DuelBattleSession.Round;

        public DuelSessionService(PlayerSquadService playerSquadService,
                                  EnemySquadService enemySquadService,
                                  BattleSessionRepository battleSessionRepository)
        {
            _enemySquadService = enemySquadService;
            _battleSessionRepository = battleSessionRepository;
            playerSquadService.ResetSquad(GameMode.Duel);
            var newDuel = DuelBattleSession.Build(GameMode.Duel, _enemySquadService);
            _battleSessionRepository.Set(newDuel);
        }

        public void StartBattle()
        {
            StartRound();
            if (DuelBattleSession.Round == 1) {
                _messenger.Publish(new DuelBattleStartMessage());
            }
        }

        public BattleSession FinishBattle()
        {
            return BattleSession;
        }

        public void IncreaseRound()
        {
            DuelBattleSession.IncreaseRound(_enemySquadService);
        }

        public void FinishBattleWithCheats(UnitType winner)
        {
            DuelBattleSession.WinRoundCount[winner] = _duelConfig.DuelWinCount;
            BattleSessionService.FinishBattleWithCheats(_unitService, winner);
        }

        private void StartRound()
        {
            _navMapService.InitMap();
            _battleConfigurationService.Configure(DuelBattleSession);
            _unitService.Init();
            _analytics.ReportDuelRoundStart(_playerProgressService.Progress.DuelProgress, Round);
            if (IsAnySquadEmpty()) {
                FinishRound(GetWinner().Value);
            } else {
                _unitService.OnUnitDeath += OnUnitDeath;
            }
            _messenger.Publish(new RoundStartMessage());
        }

        private void OnUnitDeath(Unit unit)
        {
            BattleSessionService.ReportUnitKilled(_battleSessionRepository, unit);
            var roundWinner = GetWinner();
            if (roundWinner != null) {
                FinishRound(roundWinner.Value);
            }
        }

        private void FinishRound(UnitType roundWinner)
        {
            BattleSessionService.DisposeUnits(_unitService);
            _unitService.OnUnitDeath -= OnUnitDeath;
            DuelBattleSession.IncreaseWinRoundCount(roundWinner);

            var matchWinner = DuelBattleSession.TryFindMatchWinner(_duelConfig);
            _analytics.ReportDuelRoundEnd(_playerProgressService.Progress.DuelProgress, Round, roundWinner == UnitType.PLAYER);
            if (matchWinner != null) {
                FinishMatch(matchWinner.Value);
            }
            _messenger.Publish(new RoundEndMessage() {
                    Winner = roundWinner,
                    Score = DuelBattleSession.WinRoundCount,
                    FinishedMatch = matchWinner != null,
                    EnemiesKilled = DuelBattleSession.EnemiesKilled,
            });
        }

        private void FinishMatch(UnitType winner)
        {
            DuelBattleSession.Finish(winner);
            _messenger.Publish(new DuelBattleEndMessage {
                    IsPlayerWon = winner == UnitType.PLAYER
            });
        }

        private bool IsAnySquadEmpty() => GetWinner().HasValue;
        private UnitType? GetWinner() => BattleSessionService.GetWinner(_unitService);
    }
}