using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LegionMaster.Extension;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.UI.Screen.Debriefing;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegionMaster.UI.Dialog.LootBox.View
{
    public class LootBoxRewardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private LootBoxRewardItemPanel _rewardItemPrefab;
        [SerializeField]
        private Transform _rewardItemRoot;
        [SerializeField]
        private GridLayoutGroupDecreaseScaler _groupDecreaseScaler;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private float _scrollMoveDuration = 1f;

        private CompositeDisposable _disposable;
        private bool _speedUpAnimation;
        private Action _onRewardCollectionOpened;
        private int _openedRewardCount;

        public void Init(LootBoxDialogModel model)
        {
            _groupDecreaseScaler.InitScale();
            _openedRewardCount = 0;
            _onRewardCollectionOpened = model.OnRewardCollectionOpened;
            RemoveAllCreatedObjects();
            Active = true;

            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            OpenRewardCollection(model.RewardCollection[0]);
            model.RewardCollection.ObserveAdd().Subscribe(it => OpenRewardCollection(it.Value)).AddTo(_disposable);
        }

        private void OpenRewardCollection(RewardItemCollection rewardItemCollection)
        {
            _speedUpAnimation = false;
            OpenNextRewardItem(rewardItemCollection.RewardsEnumerator);
        }

        private void OpenNextRewardItem(IEnumerator<RewardItemModel> rewards)
        {
            if (!rewards.MoveNext()) {
                _onRewardCollectionOpened?.Invoke();
                return;
            }
            StartCoroutine(CreateRewardItemPanel(rewards));
        }

        private IEnumerator CreateRewardItemPanel(IEnumerator<RewardItemModel> rewards)
        {
            var rewardItemPanel = Instantiate(_rewardItemPrefab, _rewardItemRoot, false);
            rewardItemPanel.Init(rewards.Current);

            _groupDecreaseScaler.DecreaseScale(++_openedRewardCount);
            MoveScrollDown(_scrollRect.verticalNormalizedPosition, 0, _scrollMoveDuration);

            if (_speedUpAnimation) {
                yield return rewardItemPanel.PlayOpenQuicklyAnimation();
                OpenNextRewardItem(rewards);
            }
            yield return rewardItemPanel.PlayOpenAnimation();
            OpenNextRewardItem(rewards);
        }

        private void MoveScrollDown(float fromValue, float toValue, float time) =>
                DOTween.To(() => fromValue, value => { _scrollRect.verticalNormalizedPosition = value; }, toValue, time);

        private void OnDisable()
        {
            gameObject.SetActive(false);
            RemoveAllCreatedObjects();
        }

        private void RemoveAllCreatedObjects()
        {
            _disposable?.Dispose();
            _disposable = null;
            _rewardItemRoot.DestroyAllChildren();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _speedUpAnimation = true;
        }
        public bool Active
        {
            set { gameObject.SetActive(value); }
        }
    }
}