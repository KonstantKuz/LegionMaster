using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.BattlePass.Model;
using LegionMaster.Extension;
using LegionMaster.UI.Screen.BattlePass.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.BattlePass.View
{
    public class BattlePassLevelListView : MonoBehaviour
    {
        [SerializeField] private BattlePassLevelView _levelViewPrefab;
        [SerializeField] private Transform _placementRoot;
        
        [SerializeField] private ScrollRect _scrollRect;

        private CompositeDisposable _disposable;
        private RectTransform _childScrollRect;

        public ScrollRect ScrollRect => _scrollRect;

        public void Init(BattlePassLevelListModel levelListModel)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            RemoveAllCreatedObjects();
            CreateLevelListView(levelListModel.Levels);
        }

        private void CreateLevelListView(IReadOnlyCollection<IReactiveProperty<BattlePassLevelModel>> levels)
        {
            var sortedLevels = levels.OrderByDescending(it => it.Value.LevelInfo.Level);
            
            foreach (var level in sortedLevels) {
                var levelView = Instantiate(_levelViewPrefab, _placementRoot);
                level.Subscribe(it => levelView.Init(it)).AddTo(_disposable);

                if (IsItemForScrollPosition(level.Value)) {
                    _childScrollRect = levelView.GetComponent<RectTransform>();
                }
            }
            StartCoroutine(ScrollToItemAfterLayoutUpdate());
        }

        private bool IsItemForScrollPosition(BattlePassLevelModel level)
        {
            return level.BasicReward.BattlePassReward.State == BattlePassRewardState.Unavailable
                   || level.BasicReward.BattlePassReward.State == BattlePassRewardState.Available
                   || level.PremiumReward.BattlePassReward.State == BattlePassRewardState.Available;
        }


        private IEnumerator ScrollToItemAfterLayoutUpdate()
        {
            yield return null;
            _scrollRect.ScrollToChild(_childScrollRect);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
            RemoveAllCreatedObjects();
        }

        private void RemoveAllCreatedObjects()
        {
            _placementRoot.DestroyAllChildren();
        }
    }
}