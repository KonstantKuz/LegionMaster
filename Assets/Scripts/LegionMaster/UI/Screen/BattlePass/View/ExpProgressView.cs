using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.BattlePass.Model;
using TMPro;
using UnityEngine;

namespace LegionMaster.UI.Screen.BattlePass.View
{
    public class ExpProgressView : MonoBehaviour
    {
        [SerializeField] private AnimatedIntView _level;
        [SerializeField] private ProgressBarView _expProgress;
        
        [SerializeField] private TextMeshProUGUI _currentExp;      
        [SerializeField] private TextMeshProUGUI _maxExp;
        
        public void Init(ExpProgressModel model)
        {
            UpdateExpProgress(model);
        }
        private void UpdateExpProgress(ExpProgressModel model)
        {
            var forceLoop = _level.Value != model.Level;
            _expProgress.SetValueWithLoop(model.ExpProgress, forceLoop);
            _level.SetData(model.Level);
            CurrentExp = model.CurrentExp.ToString();
            MaxExp = model.MaxExp.ToString();
        }
        private string CurrentExp
        {
            set => _currentExp.text = value;
        } 
        private string MaxExp
        {
            set => _maxExp.text = value;
        }
        private void OnDisable()
        {
            _expProgress.Reset();
            _level.Reset();
        }
    }
}