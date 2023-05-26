using System.Collections.Generic;
using System.Linq;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Event;
using LegionMaster.Config;
using LegionMaster.Player;
using LegionMaster.Player.Progress;
using LegionMaster.Player.Progress.Model;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Quest.Config;
using LegionMaster.Quest.Message;
using LegionMaster.Quest.Model;
using LegionMaster.Repository;
using LegionMaster.Reward.Config;
using LegionMaster.Reward.Service;
using ModestTree;
using SuperMaxim.Core.Extensions;
using SuperMaxim.Messaging;

namespace LegionMaster.Quest.Service
{
    public class QuestService
    {
        private PlayerProgressService _playerProgress;
        private readonly ISingleModelRepository<QuestCollection> _questRepository;
        private readonly ISingleModelRepository<QuestSectionCollection> _questSectionRepository;
        private readonly StringKeyedConfigCollection<QuestConfig> _questStringKeyedConfig;
        private readonly ConfigCollection<QuestSectionRewardId, QuestSectionRewardConfig> _sectionConfig;
        private readonly IRewardApplyService _rewardApplyService;
        private readonly Analytics.Analytics _analytics;
        private readonly ISingleModelRepository<QuestRewardList> _forgottenRewards;
        private readonly IMessenger _messenger;

        private QuestCollection _quests;
        private QuestSectionCollection _sections;

        public QuestService(ISingleModelRepository<QuestCollection> questRepository, 
            ISingleModelRepository<QuestSectionCollection> questSectionRepository, 
            ISingleModelRepository<QuestRewardList> forgottenRewards,
            StringKeyedConfigCollection<QuestConfig> questStringKeyedConfig, 
            ConfigCollection<QuestSectionRewardId, QuestSectionRewardConfig> sectionConfig,
            IRewardApplyService rewardApplyService,
            Analytics.Analytics analytics,
            IMessenger messenger, 
            PlayerProgressService playerProgressService)
        {
            _questRepository = questRepository;
            _questSectionRepository = questSectionRepository;
            _forgottenRewards = forgottenRewards;
            _questStringKeyedConfig = questStringKeyedConfig;
            _sectionConfig = sectionConfig;
            _rewardApplyService = rewardApplyService;
            _analytics = analytics;
            _messenger = messenger;
            _playerProgress = playerProgressService;
            
            Load();
        }

        private void Load()
        {
            LoadQuests();
            LoadSections();
            RestartTimedOutQuests();            
        }

        private void LoadSections()
        {
            _sections = _questSectionRepository.Get() ?? new QuestSectionCollection();
        }

        private void LoadQuests()
        {
            var storedQuests = _questRepository.Get();
            _quests = new QuestCollection();
            foreach (var config in _questStringKeyedConfig.Values)
            {
                var storedState = storedQuests?.Find(config.Id);
                var quest = new Model.Quest(config, storedState);
                _quests.Add(quest);
            }
        }

        public IReadOnlyList<Model.Quest> Quests => _quests;

        public IEnumerable<Model.Quest> GetQuestBySection(QuestSectionType type) =>
            Quests.Where(it => it.Config.Section == type);

        /// <summary>
        /// Increases counter for condition  conditionId.
        /// This method is slow cause it saves data to PlayerPrefs - do not call it during battle
        /// </summary>
        /// <param name="conditionId"></param>
        /// <param name="amount"></param>
        public void IncreaseCounter(string conditionId, int amount = 1)
        {
            foreach (var quest in _quests)
            {
                if (quest.Condition != conditionId) continue;
                var wasCompleted = quest.Completed;
                var rewardsBefore = GetReachedSectionRewards(quest);
                quest.IncreaseCounter(amount);
                ReportQuestCompletionToAnalytics(wasCompleted, quest, rewardsBefore);
                _messenger.Publish(new QuestStateChangedMessage());
            }
            SaveQuests();
        }

        private void ReportQuestCompletionToAnalytics(bool wasCompleted, Model.Quest quest, IEnumerable<QuestSectionRewardId> rewardsBefore)
        {
            if (!wasCompleted && quest.Completed)
            {
                _analytics.ReportQuestEvent(quest.Section, quest.Id, QuestEvents.COMPLETED);
            }

            var rewardsAfter = GetReachedSectionRewards(quest);
            foreach (var rewardId in rewardsAfter.Except(rewardsBefore))
            {
                _analytics.ReportQuestSectionEvent(rewardId, QuestEvents.COMPLETED);
            }
        }

        private IEnumerable<QuestSectionRewardId> GetReachedSectionRewards(Model.Quest quest)
        {
            var questSection = GetSection(quest.Section);
            return questSection.Rewards
                .Where(it => it.Config.RequiredPoints <= questSection.Points)
                .Select(it => it.Config.Id);

        }

        public void RestartTimedOutQuests()
        {
            var timedOutQuests = _quests.Where(quest => quest.TimedOut).ToList();
            if (timedOutQuests.Count == 0) return;
            var notTakenRewards = _forgottenRewards.Get() ?? new QuestRewardList();

            RestartSections(timedOutQuests, notTakenRewards);

            RestartQuests(timedOutQuests, notTakenRewards);
            
            SaveQuests();
            
            if (!notTakenRewards.IsEmpty())
            {
                _forgottenRewards.Set(notTakenRewards);
            }
        }

        private void RestartQuests(List<Model.Quest> timedOutQuests, QuestRewardList notTakenRewards)
        {
            foreach (var quest in timedOutQuests)
            {
                if (quest.Completed && !quest.RewardTaken)
                {
                    notTakenRewards.Add(quest.TakeReward());
                    _analytics.ReportQuestEvent(quest.Section, quest.Id, QuestEvents.REWARD_TAKEN);
                }

                _quests.Remove(quest);
                _quests.Add(new Model.Quest(quest.Config, null));
                _messenger.Publish(new QuestStateChangedMessage());
            }
        }

        private void RestartSections(IEnumerable<Model.Quest> timedOutQuests, QuestRewardList notTakenRewards)
        {
            var affectedSections = timedOutQuests.Select(it => it.Config.Section).Distinct().ToList();
            StoreForgottenSectionRewards(notTakenRewards, affectedSections);
            ResetSectionRewards(affectedSections);
        }

        private void StoreForgottenSectionRewards(QuestRewardList notTakenRewards, List<QuestSectionType> affectedSections)
        {
            var sectionRewards = affectedSections.SelectMany(GetNotTakenSectionReward).ToList();
            sectionRewards.ForEach(reward => _analytics.ReportQuestSectionEvent(reward.Config.Id, QuestEvents.REWARD_TAKEN));
            notTakenRewards.AddRange(sectionRewards.Select(it => it.Config.Reward));
        }

        private void ResetSectionRewards(ICollection<QuestSectionType> affectedSections)
        {
            _sections.TakenRewards.RemoveAll(reward => affectedSections.Contains(reward.Type));
        }

        private IEnumerable<QuestSection.Reward> GetNotTakenSectionReward(QuestSectionType sectionType)
        {
            var section = GetSection(sectionType);
            return section.Rewards
                .Where(reward => section.GetRewardState(reward) == QuestSection.RewardState.Completed);
        }

        public Model.Quest FindQuest(string id) => _quests.Find(id);

        public RewardItemConfig TakeQuestReward(string questId)
        {
            var quest = FindQuest(questId);
            var reward = quest?.TakeReward();
            if (reward == null) return null;
            _rewardApplyService.ApplyReward(reward.ToRewardItem());                
            SaveQuests();
            _analytics.ReportQuestEvent(quest.Section, quest.Id, QuestEvents.REWARD_TAKEN);

            _playerProgress.IncreaseCollectiblesCount(QuestTypeToCollectibles(quest.Section));

            _messenger.Publish(new QuestStateChangedMessage());
            return reward;
        }

        public void SaveQuests()
        {
            _questRepository.Set(_quests);
        }

        public IReadOnlyList<RewardItemConfig> TakeOldRewards()
        {
            var rez = _forgottenRewards.Get() ?? new QuestRewardList();
            _rewardApplyService.ApplyRewards(rez.Select(it => it.ToRewardItem()));
            _forgottenRewards.Set(new QuestRewardList());            
            return rez;
        }

        public QuestSection GetSection(QuestSectionType type)
        {
            return new QuestSection(type, GetSectionPoints(type), BuildSectionRewardList(type));
        }

        private IEnumerable<QuestSection.Reward> BuildSectionRewardList(QuestSectionType type)
        {
            return _sectionConfig.Values
                .Where(it => it.Section == type)
                .Select(it => BuildSectionReward(type, it));
        }

        private QuestSection.Reward BuildSectionReward(QuestSectionType type, QuestSectionRewardConfig config)
        {
            return new QuestSection.Reward
            {
                Config = config,
                RewardTaken = _sections.IsSectionRewardTaken(new QuestSectionRewardId
                {
                    Type = type,
                    Points = config.RequiredPoints
                })
            };
        }

        private int GetSectionPoints(QuestSectionType type)
        {
            return _quests
                .Where(it => it.Section == type)
                .Where(it => it.Completed)
                .Select(it => it.Config.Points).Sum();
        }

        private PlayerCollectibles QuestTypeToCollectibles(QuestSectionType section)
        {
            var collectible = section switch
            {
                QuestSectionType.Daily => PlayerCollectibles.QuestDaily,
                QuestSectionType.Weekly => PlayerCollectibles.QuestWeekly,
                _ => PlayerCollectibles.None
            };

            return collectible;
        }

        public RewardItemConfig TakeSectionReward(QuestSectionRewardId rewardId)
        {
            var reward = _sectionConfig.Find(rewardId);
            if (reward == null) return null;
            if (GetSectionPoints(rewardId.Type) < reward.RequiredPoints) return null;
            if (_sections.IsSectionRewardTaken(rewardId)) return null;
            _sections.TakenRewards.Add(rewardId);
            
            _questSectionRepository.Set(_sections);
            _rewardApplyService.ApplyReward(reward.Reward.ToRewardItem());
            _analytics.ReportQuestSectionEvent(rewardId, QuestEvents.REWARD_TAKEN);
            _messenger.Publish(new QuestSectionRewardTakenMessage());
            return reward.Reward;
        }

        public void TimeoutQuests(QuestSectionType type)
        {
            _quests.Where(it => it.Section == type).ForEach(it => it.MoveToPast());
            RestartTimedOutQuests();
            SaveQuests();
        }
    }
}