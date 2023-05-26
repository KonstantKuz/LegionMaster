using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.Quest.Config;
using LegionMaster.Reward.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace LegionMaster.Quest.Model
{
    [DataContract]
    public class Quest
    {
        [DataMember]
        public readonly string Id;

        [DataMember(Name = "Counter")]
        public int Counter { get; private set; }

        [DataMember(Name = "Completed")]
        public bool Completed { get; private set; }

        [DataMember(Name = "RewardTaken")]
        public bool RewardTaken { get; private set; }

        [DataMember(Name = "StartTime")]
        public DateTime StartTime { get; private set; }

        public readonly QuestConfig Config;

        public string Condition => Config.Condition;

        [JsonConstructor]
        private Quest()
        {
            //no config constructor only for temporary objects from serialization
        }

        public Quest(QuestConfig config, Quest storedState)
        {
            Config = config;
            Id = config.Id;
            StartTime = storedState?.StartTime ?? GetStartOfCurrentPeriod(config.Section);
            Counter = storedState?.Counter ?? 0;
            Completed = storedState?.Completed ?? false;
            RewardTaken = storedState?.RewardTaken ?? false;
        }

        public static DateTime GetStartOfCurrentPeriod(QuestSectionType section)
        {
            return section switch
            {
                QuestSectionType.Daily => DateTime.Today,
                QuestSectionType.Weekly => FirstDayOfCurrentWeek,
                _ => throw new ArgumentOutOfRangeException(nameof(section), section, null)
            };
        }

        public static DateTime GetEndOfCurrentPeriod(QuestSectionType section)
        {
            var start = GetStartOfCurrentPeriod(section);
            return section switch
            {
                QuestSectionType.Daily => start.AddDays(1),
                QuestSectionType.Weekly => start.AddDays(7),
                _ => throw new ArgumentOutOfRangeException(nameof(section), section, null)
            };
        }

        private static DateTime FirstDayOfCurrentWeek
        {
            get
            {
                var day = DateTime.Today;
                while (day.DayOfWeek != DayOfWeek.Monday)
                {
                    day = day.AddDays(-1);
                }

                return day;
            }
        }

        public void IncreaseCounter(int amount = 1)
        {
            if (Counter >= Config.ConditionCount) return;
            Counter = Mathf.Min(Config.ConditionCount, Counter + amount);
            if (Counter >= Config.ConditionCount) Completed = true;
        }

        public void MoveToPast()
        {
            StartTime = Config.Section switch
            {
                QuestSectionType.Daily => StartTime.AddDays(-1),
                QuestSectionType.Weekly => StartTime.AddDays(-7),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool TimedOut => StartTime < GetStartOfCurrentPeriod(Config.Section);
        public QuestSectionType Section => Config.Section;

        public RewardItemConfig TakeReward()
        {
            if (!Completed || RewardTaken) return null;
            RewardTaken = true;
            return Config.Reward;
        }
    }
    
    [DataContract]
    public class QuestCollection: List<Quest>
    {
        public Quest Find(string id) => Find(it => it.Id == id);
    }
}