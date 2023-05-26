using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace LegionMaster.UI.Screen.BattlePass.Model
{
    public class BattlePassLevelListModel
    {
        private IReadOnlyCollection<ReactiveProperty<BattlePassLevelModel>> _levels;
        
        public IReadOnlyCollection<IReactiveProperty<BattlePassLevelModel>> Levels => _levels;

        public BattlePassLevelListModel(IEnumerable<BattlePassLevelModel> levels)
        {
            _levels = levels.Select(it => new ReactiveProperty<BattlePassLevelModel>(it)).ToList();
        }
        public void UpdateLevel(BattlePassLevelModel level)
        {
            var levelProperty = _levels.First(it => it.Value.LevelInfo.Level == level.LevelInfo.Level);
            levelProperty.SetValueAndForceNotify(level);
        }
    }
}