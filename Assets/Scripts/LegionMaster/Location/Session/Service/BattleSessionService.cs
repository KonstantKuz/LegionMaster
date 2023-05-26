using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;
using LegionMaster.Extension;
using LegionMaster.Location.Session.Messages;
using LegionMaster.Location.Session.Model;
using LegionMaster.NavMap.Service;
using LegionMaster.Repository;
using LegionMaster.Units;
using LegionMaster.Units.Component;
using LegionMaster.Units.Component.Target;
using LegionMaster.Units.Service;
using SuperMaxim.Core.Extensions;
using SuperMaxim.Messaging;
using Zenject;

namespace LegionMaster.Location.Session.Service
{
    [PublicAPI]
    public class BattleSessionService : IBattleSessionService
    {
        private readonly BattleSessionRepository _battleSessionRepository;      
        private readonly EnemySquadService _enemySquadService;
        private readonly GameMode _gameMode;
        
        [Inject] private BattleConfigurationService _battleConfigurationService;
        [Inject] private UnitService _unitService;
        [Inject] private IMessenger _messenger;
        [Inject] private NavMapService _navMapService;

        public BattleSession BattleSession => _battleSessionRepository.Require();

        public BattleSessionService(GameMode gameMode, BattleSessionRepository battleSessionRepository, EnemySquadService enemySquadService)
        {
            _gameMode = gameMode;
            _battleSessionRepository = battleSessionRepository;
            _enemySquadService = enemySquadService;
            var battleSession = BattleSession.Build(gameMode, _enemySquadService);
            _battleSessionRepository.Set(battleSession);
        }

        public void StartBattle()
        {
            _navMapService.InitMap();
            _battleConfigurationService.Configure(BattleSession);
            _unitService.Init();
            _unitService.OnUnitDeath += OnUnitDeath;
            _messenger.Publish(new BattleStartMessage() {
                    GameMode = _gameMode,
            });
        }

        public BattleSession FinishBattle()
        {
            return BattleSession;
        }

        public void FinishBattleWithCheats(UnitType winner) => FinishBattleWithCheats(_unitService, winner);
        public static void FinishBattleWithCheats(UnitService unitService, UnitType winner)
        {
            new List<Unit>(unitService.Units).Where(unit => unit.UnitType != winner)
                                             .ForEach(unit => unit.Kill());
        }
        public static void ReportUnitKilled(BattleSessionRepository repository, Unit unit)
        {
            var unitType = unit.GetComponent<ITarget>().UnitType;
            if (unitType != UnitType.AI) {
                return;
            }
            repository.Require().IncreaseEnemiesKilled();
        }
        public static UnitType? GetWinner(UnitService unitService)
        {
            foreach (var winner in EnumExt.Values<UnitType>()) {
                if (unitService.AllAliveUnitsHaveType(winner)) {
                    return winner;
                }
            }
            return null;
        }
        public static void DisposeUnits(UnitService unitService) => unitService.Term();
        private void OnUnitDeath(Unit unit)
        {
            ReportUnitKilled(_battleSessionRepository, unit);
            var winner = GetWinner(_unitService);
            if (winner != null) {
                EndBattle(winner.Value);
                return;
            }
        }
        private void EndBattle(UnitType winner)
        {
            DisposeUnits(_unitService);
            _unitService.OnUnitDeath -= OnUnitDeath;
            BattleSession.SetWinnerByUnitType(winner);
            _messenger.Publish(new BattleEndMessage {
                    GameMode = _gameMode,
                    BattleResult = winner == UnitType.PLAYER ? BattleResult.WIN : BattleResult.LOSE, 
                    EnemiesKilled = _battleSessionRepository.Require().EnemiesKilled
            });
        }
    }
}