using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;
using LegionMaster.Location.Session.Model;
using LegionMaster.Units.Component;

namespace LegionMaster.Campaign.Session.Model
{
    public class CampaignBattleSession : BattleSession
    {
        private readonly EnemySquadService _enemySquadService;
        public readonly int Chapter;
        public int Stage { get; private set; }
        public int LastStageNumber => _enemySquadService.GetMatch(Mode).RoundCount;
        private CampaignBattleSession(GameMode mode, EnemySquadService enemySquadService) : base(mode, enemySquadService.GetMatchEnemySquad(mode, 1))
        {
            _enemySquadService = enemySquadService;
            Chapter = enemySquadService.GetMatch(mode).Match;
            Stage = 1;
        }

        public static CampaignBattleSession Build(GameMode mode, EnemySquadService enemySquadService)
        {
            return new CampaignBattleSession(mode, enemySquadService);
        }

        public void Finish(UnitType winner)
        {
            SetWinnerByUnitType(winner);
        }

        public UnitType? TryFindChapterWinner(UnitType stageWinner)
        {
            if (stageWinner == UnitType.AI) {
                return UnitType.AI;
            }
            if (Stage >= LastStageNumber) {
                return UnitType.PLAYER;
            }
            return null;
        }

        public void FinishWithCheats()
        {
            Stage = LastStageNumber;
        }
        public void IncreaseStage()
        {
            ++Stage;
            EnemySquad = _enemySquadService.GetMatchEnemySquad(Mode, Stage);
            EnemiesKilled = 0;
        }

    }
}