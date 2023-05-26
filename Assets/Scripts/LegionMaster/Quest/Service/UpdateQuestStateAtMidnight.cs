using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace LegionMaster.Quest.Service
{
    [PublicAPI]
    public class UpdateQuestStateAtMidnight
    {
        private readonly QuestService _questService;
        
        public UpdateQuestStateAtMidnight(QuestService questService)
        {
            _questService = questService;
            var timeTillMidnight = DateTime.Today.AddDays(1) - DateTime.Now;
            Observable.Timer(timeTillMidnight + TimeSpan.FromSeconds(1), TimeSpan.FromDays(1))
                .Subscribe(it => UpdateQuests());
        }

        private void UpdateQuests()
        {
            Debug.Log("Update quest system after midnight");
            _questService.RestartTimedOutQuests();
        }
    }
}