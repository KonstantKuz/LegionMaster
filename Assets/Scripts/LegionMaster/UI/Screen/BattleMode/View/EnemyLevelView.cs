using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.BattleMode.Model;
using UnityEngine;

namespace LegionMaster.UI.Screen.BattleMode.View
{
    public class EnemyLevelView : MonoBehaviour
    {
        public TextMeshProLocalization _levelText; 
        public ActivatableObject _levelContainer;
        public ActivatableObject _allLevelsCompletedContainer;   
        
        
        public void Init(EnemyLevelModel model)
        {
            _levelText.SetTextFormatted(_levelText.LocalizationId, model.Level);
            _levelContainer.Init(!model.AllLevelsCompleted);
            _allLevelsCompletedContainer.Init(model.AllLevelsCompleted);

        }
    }
}