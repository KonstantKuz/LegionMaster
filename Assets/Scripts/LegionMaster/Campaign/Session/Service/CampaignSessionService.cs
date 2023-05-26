using LegionMaster.Campaign.Session.Messages;
using LegionMaster.Campaign.Session.Model;
using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;
using LegionMaster.Location.Arena;
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

namespace LegionMaster.Campaign.Session.Service
{
    public class CampaignSessionService : IBattleSessionService
    {
        private readonly BattleSessionRepository _battleSessionRepository;
        private readonly LocationArena _locationArena;
        
        [Inject]
        private BattleConfigurationService _battleConfigurationService;
        [Inject]
        private UnitService _unitService;
        [Inject]
        private IMessenger _messenger;
        [Inject]
        private NavMapService _navMapService;
        [Inject]
        private Analytics.Analytics _analytics;
        [Inject]
        private PlayerProgressService _playerProgressService;
        
        public BattleSession BattleSession => _battleSessionRepository.Require();
        public CampaignBattleSession CampaignBattleSession => (CampaignBattleSession) BattleSession;
        public int Stage => CampaignBattleSession.Stage;
        public int LastStage => CampaignBattleSession.LastStageNumber;
        public bool IsLastStage => Stage == LastStage;

        public CampaignSessionService(PlayerSquadService playerSquadService,
                                      EnemySquadService enemySquadService,
                                      BattleSessionRepository battleSessionRepository, LocationArena locationArena)
        {
            _battleSessionRepository = battleSessionRepository;
            _locationArena = locationArena;
            
            playerSquadService.ResetSquad(GameMode.Campaign);
            var сhapter = CampaignBattleSession.Build(GameMode.Campaign, enemySquadService);
            _battleSessionRepository.Set(сhapter);
            _locationArena.CampaignStageDoors.Close();
            _locationArena.RewardChest.Close();
        }

        public void IncreaseStage()
        {
            CampaignBattleSession.IncreaseStage();
            _locationArena.CampaignStageDoors.Close();
            if (IsLastStage)
            {
                _locationArena.RewardChest.Show();
            }
        }

        public void StartBattle()
        {
            StartStage();
            if (CampaignBattleSession.Stage == 1) {
                _messenger.Publish(new CampaignBattleStartMessage());
            }
        }

        public BattleSession FinishBattle()
        {
            _locationArena.RewardChest.Hide();
            return BattleSession;
        }

        public void FinishBattleWithCheats(UnitType winner)
        {
            CampaignBattleSession.FinishWithCheats();
            BattleSessionService.FinishBattleWithCheats(_unitService, winner);
        }

        private void StartStage()
        {
            _navMapService.InitMap();
            _battleConfigurationService.Configure(CampaignBattleSession);
            _unitService.Init();
            _analytics.ReportCampaignStageStart(_playerProgressService.Progress.CampaignProgress, Stage);
            if (IsAnySquadEmpty()) {
                FinishStage(GetWinner().Value);
            } else {
                _unitService.OnUnitDeath += OnUnitDeath;
            }
            _messenger.Publish(new StageStartMessage());
        }

        private void OnUnitDeath(Unit unit)
        {
            BattleSessionService.ReportUnitKilled(_battleSessionRepository, unit);
            var stageWinner = GetWinner();
            if (stageWinner != null) {
                FinishStage(stageWinner.Value);
            }
        }

        private void FinishStage(UnitType stageWinner)
        {
            BattleSessionService.DisposeUnits(_unitService);
            _unitService.OnUnitDeath -= OnUnitDeath;

            var matchWinner = CampaignBattleSession.TryFindChapterWinner(stageWinner);
            _analytics.ReportCampaignStageEnd(_playerProgressService.Progress.CampaignProgress, Stage, stageWinner == UnitType.PLAYER);
            if (matchWinner != null) {
                FinishMatch(matchWinner.Value);
            }
            _messenger.Publish(new StageEndMessage() {
                    Winner = stageWinner,
                    Stage = Stage,
                    FinishedMatch = matchWinner != null,
                    EnemiesKilled = CampaignBattleSession.EnemiesKilled,
            });
        }

        private void FinishMatch(UnitType winner)
        {
            CampaignBattleSession.Finish(winner);
            _messenger.Publish(new CampaignBattleEndMessage {
                    IsPlayerWon = winner == UnitType.PLAYER
            });
        }

        private bool IsAnySquadEmpty() => GetWinner().HasValue;
        private UnitType? GetWinner() => BattleSessionService.GetWinner(_unitService);
    }
}