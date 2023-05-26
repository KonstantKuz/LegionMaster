using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Config;
using LegionMaster.Enemy.Service;
using LegionMaster.Units.Component;

namespace LegionMaster.Location.Session.Model
{
    public class BattleSession
    {
        public readonly GameMode Mode;

        public EnemySquadConfig EnemySquad { get; protected set; }
        public int BattleId { get; private set; }
        public int EnemiesKilled { get; protected set; }
        public BattleResult? Winner { get; private set; }

        protected BattleSession(GameMode mode, EnemySquadConfig enemySquad)
        {
            Mode = mode;
            EnemySquad = enemySquad;
        }

        public static BattleSession Build(GameMode mode, EnemySquadService enemySquadService)
        {
            var session = new BattleSession(mode, enemySquadService.GetEnemySquad(mode)) {
                    BattleId = enemySquadService.GetEnemySquadId(mode)
            };
            return session;
        }

        public void IncreaseEnemiesKilled() => ++EnemiesKilled;

        public void SetWinnerByUnitType(UnitType unitType)
        {
            Winner = unitType == UnitType.PLAYER ? BattleResult.WIN : BattleResult.LOSE;
        }
    }
}