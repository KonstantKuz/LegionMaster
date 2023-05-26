using System;
using LegionMaster.Quest.Config;
using LegionMaster.Quest.Model;
using UnityEngine;

namespace LegionMaster.UI.Screen.Quest
{
    public class SectionProgressModel
    {
        public struct RewardItemModel
        {
            public QuestSection.RewardState State;
            public QuestSectionRewardConfig Config;
            public Action<Transform> ClickAction;
        }
        
        public int Exp;
        public float Progress;
        public RewardItemModel[] Rewards;
    }
}