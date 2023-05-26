using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Analytics;
using LegionMaster.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Progress.Config;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Quest.Config;
using LegionMaster.Quest.Model;
using LegionMaster.Quest.Service;
using LegionMaster.Repository;
using LegionMaster.Reward.Config;
using LegionMaster.Reward.Model;
using LegionMaster.Reward.Service;
using NSubstitute;
using NUnit.Framework;
using SuperMaxim.Messaging;
using Zenject;

namespace Tests.Quest
{
    [TestFixture]
    public class QuestTest : ZenjectUnitTestFixture
    {
        public const string QUEST_A = "questA";
        private const string QUEST_B = "questB";
        private const string QUEST_C = "questC";
        private const string KILL_CONDITION = "KillEnemy";

        public class QuestRepository : SingleModelRepository<QuestCollection>
        {
        }

        public class QuestSectionRepository : SingleModelRepository<QuestSectionCollection>
        {
        }

        public static ConfigCollection<QuestSectionRewardId, QuestSectionRewardConfig> CreateSectionConfig()
        {
            return new ConfigCollection<QuestSectionRewardId, QuestSectionRewardConfig>(new []
            {
                    new QuestSectionRewardConfig
                    {
                            RequiredPoints = 10,
                            Reward = new RewardItemConfig
                            {
                                    Type = RewardType.Currency,
                                    Id = Currency.Soft.ToString(),
                                    Count = 1000
                            },
                            Section = QuestSectionType.Daily
                    }
            });
        }

        public static StringKeyedConfigCollection<QuestConfig> CreateQuestConfig()
        {
            return new StringKeyedConfigCollection<QuestConfig>(new []
            {
                    new QuestConfig
                    {
                            Id = QUEST_A,
                            Section = QuestSectionType.Daily,
                            Condition = KILL_CONDITION,
                            ConditionCount = 3,
                            Points = 10,
                            Reward = new RewardItemConfig
                            {
                                    Type = RewardType.Currency,
                                    Id = Currency.Soft.ToString(),
                                    Count = 50
                            }
                    },
                    new QuestConfig
                    {
                            Id = QUEST_B,
                            Section = QuestSectionType.Daily,
                            Condition = "PlantTree",
                            ConditionCount = 3,
                            Points = 10,
                            Reward = new RewardItemConfig
                            {
                                    Type = RewardType.Currency,
                                    Id = Currency.Soft.ToString(),
                                    Count = 50
                            }
                    },
                    new QuestConfig
                    {
                            Id = QUEST_C,
                            Section = QuestSectionType.Weekly,
                            Condition = KILL_CONDITION,
                            ConditionCount = 5,
                            Points = 10,
                            Reward = new RewardItemConfig
                            {
                                    Type = RewardType.Currency,
                                    Id = Currency.Soft.ToString(),
                                    Count = 50
                            }
                    }
            });
        }

        [Test]
        public void GetQuests()
        {
            var service = CreateService();
            var quests = service.Quests;
            Assert.That(quests.Count, Is.EqualTo(3));
            Assert.That(quests[0].Id, Is.EqualTo(QUEST_A));
        }

        public static QuestService CreateService(ISingleModelRepository<QuestCollection> questRepository = null, 
            ISingleModelRepository<QuestSectionCollection> questSectionRepository = null, 
            StringKeyedConfigCollection<QuestConfig> questStringKeyedConfig = null, 
            ConfigCollection<QuestSectionRewardId, QuestSectionRewardConfig> sectionConfig = null,
            IRewardApplyService rewardApplyService = null)
        {
            var messenger = Substitute.For<IMessenger>();
            var analytics = Substitute.For<Analytics>(new List<IAnalyticsImpl>());
            return new QuestService(questRepository ?? new QuestRepository(), 
                questSectionRepository ?? new QuestSectionRepository(), 
                new SingleModelRepository<QuestRewardList>(),
                questStringKeyedConfig ?? CreateQuestConfig(), 
                sectionConfig ?? CreateSectionConfig(), 
                rewardApplyService ?? Substitute.For<IRewardApplyService>(),
                analytics,
                messenger,
                Substitute.For<PlayerProgressService>(
                    messenger, 
                    Substitute.For<PlayerProgressRepository>(),
                    analytics,
                    Substitute.For<StringKeyedConfigCollection<LevelConfig>>()));
        }

        [Test]
        public void IncreaseCounter()
        {
            var service = CreateService();
            var questA = service.FindQuest(QUEST_A);
            var questB = service.FindQuest(QUEST_B);
            var questC = service.FindQuest(QUEST_C);
            Assert.That(questA.Counter, Is.EqualTo(0));
            
            service.IncreaseCounter(KILL_CONDITION);
            Assert.That(questA.Counter, Is.EqualTo(1));
            Assert.That(questB.Counter, Is.EqualTo(0));
            Assert.That(questC.Counter, Is.EqualTo(1));
            
            service.IncreaseCounter("DoSomething");
            Assert.That(questA.Counter, Is.EqualTo(1));
            
            service.IncreaseCounter(KILL_CONDITION);
            service.IncreaseCounter(KILL_CONDITION);
            Assert.That(questA.Counter, Is.EqualTo(3));
            
            service.IncreaseCounter(KILL_CONDITION);
            Assert.That(questA.Counter, Is.EqualTo(3));
        }

        [Test]
        public void PersistQuestState()
        {
            var questRepository = new QuestRepository();
            var sectionRepository = new QuestSectionRepository();
            CompleteQuestA(questRepository, sectionRepository);

            var service = CreateService(questRepository, sectionRepository);
            var quest = service.FindQuest(QUEST_A);
            Assert.That(quest.Counter, Is.EqualTo(3));
            Assert.That(quest.Completed, Is.True);
        }

        public static void Kill3Enemies(QuestService service)
        {
            service.IncreaseCounter(KILL_CONDITION);
            service.IncreaseCounter(KILL_CONDITION);
            service.IncreaseCounter(KILL_CONDITION);
        }

        [Test]
        public void MarkCompleted()
        {
            var service = CreateService();
            var quest = service.FindQuest(QUEST_A);
            Assert.That(quest.Completed, Is.False);
            Kill3Enemies(service);
            Assert.That(quest.Completed, Is.True);
        }

        [Test]
        public void StartTime()
        {
            var service = CreateService();
            var dailyQuest = service.FindQuest(QUEST_A);
            Assert.That(dailyQuest.StartTime, Is.EqualTo(DateTime.Today));

            var weeklyQuest = service.FindQuest(QUEST_C);
            Assert.That(weeklyQuest.StartTime.DayOfWeek == DayOfWeek.Monday);
        }
        

        [Test]
        public void MoveToPast()
        {
            var service = CreateService();
            var dailyQuest = service.FindQuest(QUEST_A);
            Assert.That(dailyQuest.StartTime, Is.EqualTo(DateTime.Today));
            dailyQuest.MoveToPast();
            Assert.That(dailyQuest.StartTime, Is.EqualTo(DateTime.Today.AddDays(-1)));
            
            var weeklyQuest = service.FindQuest(QUEST_C);
            Assert.That(weeklyQuest.StartTime, Is.EqualTo(LegionMaster.Quest.Model.Quest.GetStartOfCurrentPeriod(QuestSectionType.Weekly)));
            weeklyQuest.MoveToPast();
            Assert.That(weeklyQuest.StartTime, Is.Not.EqualTo(LegionMaster.Quest.Model.Quest.GetStartOfCurrentPeriod(QuestSectionType.Weekly)));
        }

        [Test]
        public void RestartQuests()
        {
            var service = CreateService();
            var questA = service.Quests[0];
            service.IncreaseCounter(KILL_CONDITION);
            questA.MoveToPast();

            service.RestartTimedOutQuests();
            var newQuestA = service.FindQuest(questA.Id);
            Assert.That(newQuestA.Counter, Is.EqualTo(0));
        }

        [Test]
        public void TakeRewardForQuest()
        {
            var rewardApplyService = Substitute.For<IRewardApplyService>();
            
            var service = CreateService(rewardApplyService: rewardApplyService);
            var questA = service.FindQuest(QUEST_A);
            Assert.That(service.TakeQuestReward(questA.Id), Is.Null);
            
            Kill3Enemies(service);

            var reward = service.TakeQuestReward(questA.Id);
            Assert.That(reward.Id, Is.EqualTo(Currency.Soft.ToString()));
            Assert.That(reward.Count, Is.EqualTo(50));
            Assert.That(questA.RewardTaken, Is.True);
            
            rewardApplyService.Received().ApplyReward(new RewardItem(Currency.Soft.ToString(), RewardType.Currency, 50));
        }

        [Test]
        public void StoreNotTakenRewards()
        {
            var rewardApplyService = Substitute.For<IRewardApplyService>();
            var service = CreateService(rewardApplyService: rewardApplyService);
            var questA = service.Quests[0];
            
            Kill3Enemies(service);
            
            questA.MoveToPast();
            service.RestartTimedOutQuests();

            var rewards = service.TakeOldRewards();
            Assert.That(rewards.Count, Is.GreaterThanOrEqualTo(1));
            var expectedReward = new RewardItem(Currency.Soft.ToString(), RewardType.Currency, 50);
            Assert.That(rewards.Select(it => it.ToRewardItem()), Has.Member(expectedReward));
            
            rewardApplyService.Received().ApplyRewards( IsArrayContains(expectedReward));
            
            Assert.That(service.TakeOldRewards().Count, Is.EqualTo(0));
        }

        private static IEnumerable<T> IsArrayContains<T>(T item)
        {
            return Arg.Is<IEnumerable<T>>(rewards => 
                rewards.Contains(item));
        }

        [Test]
        public void AddNewQuestToConfig()
        {
            var questRepository = new QuestRepository();
            var sectionRepository = new QuestSectionRepository();
            CompleteQuestA(questRepository, sectionRepository);

            const string newQuestId = "newQuest";
            var newConfig = new StringKeyedConfigCollection<QuestConfig>(new []{new QuestConfig
            {
                Id = newQuestId,
                Section = QuestSectionType.Weekly,
                Condition = KILL_CONDITION,
                ConditionCount = 15,
                Points = 20,
                Reward = new RewardItemConfig
                {
                    Type = RewardType.Currency,
                    Id = Currency.Soft.ToString(),
                    Count = 50
                }
            }}.Concat(CreateQuestConfig().Values).ToList());
            var service = CreateService(questRepository, sectionRepository, newConfig);
            var questA = service.FindQuest(QUEST_A);
            Assert.That(questA.Completed, Is.True);
            var newQuest = service.FindQuest(newQuestId);
            Assert.That(newQuest, Is.Not.Null);
        }

        private static void CompleteQuestA(QuestRepository questRepository, QuestSectionRepository sectionRepository)
        {
            var service = CreateService(questRepository, sectionRepository);
            Kill3Enemies(service);
            service.SaveQuests();
        }

        [Test]
        public void ChangeTestConfig()
        {
            var questRepository = new QuestRepository();
            var sectionRepository = new QuestSectionRepository();
            CompleteQuestA(questRepository, sectionRepository);
            
            var newConfig = new StringKeyedConfigCollection<QuestConfig>(new []{new QuestConfig
            {
                Id = QUEST_A,
                Section = QuestSectionType.Daily,
                Condition = KILL_CONDITION,
                ConditionCount = 15,
                Points = 20,
                Reward = new RewardItemConfig
                {
                    Type = RewardType.Currency,
                    Id = Currency.Soft.ToString(),
                    Count = 50
                }
            }});
            var service = CreateService(questRepository, sectionRepository, newConfig);
            var questA = service.FindQuest(QUEST_A);
            Assert.That(questA.Completed, Is.True);
            Assert.That(questA.Counter, Is.EqualTo(3));
            Assert.That(questA.Config.ConditionCount, Is.EqualTo(15));
        }
    }
}