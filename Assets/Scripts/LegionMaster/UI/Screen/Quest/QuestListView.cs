using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Quest
{
    public class QuestListView : MonoBehaviour
    {
        [SerializeField] private QuestItemView _itemPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [Inject] private DiContainer _container;

        public void Init(IEnumerable<QuestItemModel> items)
        {
            RemoveAllChildren();
            _scrollRect.normalizedPosition = Vector2.up;
            foreach (var item in items.OrderBy(it => GetStateOrder(it.State)))
            {
                var itemView = _container.InstantiatePrefabForComponent<QuestItemView>(_itemPrefab, transform);
                itemView.Init(item);
            }
        }

        private static int GetStateOrder(QuestItemModel.QuestState state)
        {
            return state switch
            {
                QuestItemModel.QuestState.Active => 1,
                QuestItemModel.QuestState.Completed => 0,
                QuestItemModel.QuestState.RewardTaken => 2,
                _ => 3
            };
        }
        private void RemoveAllChildren()
        {
            foreach (var child in GetComponentsInChildren<QuestItemView>())
            {
                Destroy(child.gameObject);
            }
        }
    }
}