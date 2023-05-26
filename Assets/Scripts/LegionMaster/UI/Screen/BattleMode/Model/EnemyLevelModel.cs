using LegionMaster.Core.Mode;
using LegionMaster.Enemy.Service;

namespace LegionMaster.UI.Screen.BattleMode.Model
{
    public class EnemyLevelModel
    {
        public int Level { get; private set; }
        public bool AllLevelsCompleted { get; private set; }
        
        public static EnemyLevelModel CreateForBattle(EnemySquadService enemySquad) =>
                new EnemyLevelModel() {
                        Level = enemySquad.GetEnemySquadId(GameMode.Battle),
                        AllLevelsCompleted = !enemySquad.BattleSquadExists,
                };

        public static EnemyLevelModel CreateForHyperCasual(EnemySquadService enemySquad) =>
            new EnemyLevelModel()
            {
                Level = enemySquad.GetEnemySquadId(GameMode.HyperCasual),
                AllLevelsCompleted = false,
            };
    }
}