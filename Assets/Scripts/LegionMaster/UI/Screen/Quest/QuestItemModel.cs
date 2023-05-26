using System;
using LegionMaster.Reward.Model;
using UnityEngine;

namespace LegionMaster.UI.Screen.Quest
{
    public class QuestItemModel
    {
        public enum QuestState
        {
            Active,
            Completed,
            RewardTaken
        }
        public string LocalizationId;
        public int ExpReward;
        public int Count;
        public int MaxCount;
        public RewardItem Reward;
        public QuestState State;
        public Action<Transform> ClickAction;
    }
}