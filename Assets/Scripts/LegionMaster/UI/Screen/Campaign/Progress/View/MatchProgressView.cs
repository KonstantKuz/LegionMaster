using LegionMaster.Extension;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Campaign.Progress.Model;
using LegionMaster.Util.SerializableDictionary;
using UnityEngine;

namespace LegionMaster.UI.Screen.Campaign.Progress.View
{
    public class MatchProgressView : MonoBehaviour
    {
        [SerializeField]
        private Transform _root;
        [SerializeField]
        private StageView _stagePrefab;
        [SerializeField]
        private TextMeshProLocalization _stageText;
        [SerializeField]
        private SerializableDictionary<StageState, Sprite> _sprites;
        
        public void Init(MatchProgressModel model)
        {
            _root.DestroyAllChildren();
            _stageText.SetTextFormatted(_stageText.LocalizationId, model.CurrentStage);
            for (int i = 1; i < model.StageCount + 1; i++) {
                CreateStageView(i, model);
            }
        }
        private void CreateStageView(int index, MatchProgressModel model)
        {
            var stageView = Instantiate(_stagePrefab, _root);
            var sprite = _sprites[GetStageState(index, model)];
            stageView.Init(sprite, model.StageCount == index);
        }
        private static StageState GetStageState(int index, MatchProgressModel model)
        {
            if (index < model.CurrentStage) {
                return StageState.Completed;
            }
            return index == model.CurrentStage ? StageState.Current : StageState.Coming;
        }

        private void OnDisable()
        {
            _root.DestroyAllChildren();
        }
    }
}