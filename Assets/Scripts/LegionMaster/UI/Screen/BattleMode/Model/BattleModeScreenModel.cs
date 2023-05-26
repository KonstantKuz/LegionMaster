using LegionMaster.Enemy.Service;

namespace LegionMaster.UI.Screen.BattleMode.Model
{
    public class BattleModeScreenModel
    {
        private readonly EnemySquadService _enemySquad;
    
        public readonly EnemyLevelModel EnemyLevel;
        public bool CanStartBattle => _enemySquad.BattleSquadExists;
        public BattleModeScreenModel(EnemySquadService enemySquad)
        {
            _enemySquad = enemySquad;
            EnemyLevel = EnemyLevelModel.CreateForBattle(enemySquad);
        }
    }
}