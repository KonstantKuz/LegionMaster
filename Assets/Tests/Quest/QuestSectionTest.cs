using System.Linq;
using LegionMaster.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Quest.Config;
using LegionMaster.Quest.Model;
using LegionMaster.Reward.Config;
using LegionMaster.Reward.Model;
using LegionMaster.Reward.Service;
using NSubstitute;
using NUnit.Framework;
using Zenject;

namespace Tests.Quest
{
    [TestFixture]
    public class QuestSectionTest : ZenjectUnitTestFixture
    {
        private const int SECTION_POINTS = 10;
        
        [Test]
        public void SectionPoints()
        {
            var service = QuestTest.CreateService();
            var section = service.GetSection(QuestSectionType.Daily);
            Assert.That(section.Points, Is.EqualTo(0));
            
            QuestTest.Kill3Enemies(service);
            
            section = service.GetSection(QuestSectionType.Daily);
            Assert.That(section.Points, Is.EqualTo(10));
        }

        [Test]
        public void TakeReward()
        {
            var rewardApplyService = Substitute.For<IRewardApplyService>();
            var service = QuestTest.CreateService(rewardApplyService: rewardApplyService);
            var reward = service.TakeSectionReward(new QuestSectionRewardId(QuestSectionType.Daily, SECTION_POINTS));
            Assert.That(reward, Is.Null);
            
            QuestTest.Kill3Enemies(service);
            reward = service.TakeSectionReward(new QuestSectionRewardId(QuestSectionType.Daily, SECTION_POINTS));
            Assert.That(reward.Id, Is.EqualTo(Currency.Soft.ToString()));
            Assert.That(reward.Count, Is.EqualTo(1000));
            
            rewardApplyService.Received().ApplyReward(new RewardItem(Currency.Soft.ToString(), RewardType.Currency, 1000));

            var section = service.GetSection(QuestSectionType.Daily);
            Assert.That(section.Rewards.First(it => it.Config.RequiredPoints == SECTION_POINTS).RewardTaken, Is.True);
            
            reward = service.TakeSectionReward(new QuestSectionRewardId(QuestSectionType.Daily, SECTION_POINTS));
            Assert.That(reward, Is.Null);
        }

        [Test]
        public void PersistSectionState()
        {
            var questRepository = new QuestTest.QuestRepository();
            var sectionRepository = new QuestTest.QuestSectionRepository();
            Take10PointReward(questRepository, sectionRepository);

            var service = QuestTest.CreateService(questRepository, sectionRepository);
            var section = service.GetSection(QuestSectionType.Daily);
            Assert.That(section.Points, Is.EqualTo(10));
            Assert.That(section.FindReward(SECTION_POINTS)?.RewardTaken, Is.True);
        }

        private static void Take10PointReward(QuestTest.QuestRepository questRepository, QuestTest.QuestSectionRepository sectionRepository)
        {
            var service = QuestTest.CreateService(questRepository, sectionRepository);
            QuestTest.Kill3Enemies(service);
            service.TakeSectionReward(new QuestSectionRewardId(QuestSectionType.Daily, SECTION_POINTS));
            service.SaveQuests();
        }

        [Test]
        public void AddNewReward()
        {
            var questRepository = new QuestTest.QuestRepository();
            var sectionRepository = new QuestTest.QuestSectionRepository();
            Take10PointReward(questRepository, sectionRepository);
            
            var service = QuestTest.CreateService(questRepository, sectionRepository, QuestTest.CreateQuestConfig(), new ConfigCollection<QuestSectionRewardId, QuestSectionRewardConfig>(
                new []
                {
                    new QuestSectionRewardConfig
                    {
                        RequiredPoints = 20,
                        Reward = new RewardItemConfig
                        {
                            Type = RewardType.Currency,
                            Id = Currency.Soft.ToString(),
                            Count = 1000
                        },
                        Section = QuestSectionType.Daily
                    }
                }.Concat(QuestTest.CreateSectionConfig().Values).ToList()));
            var section = service.GetSection(QuestSectionType.Daily);
            Assert.That(section.Rewards.Count, Is.EqualTo(2));
            Assert.That(section.FindReward(SECTION_POINTS), Is.Not.Null);
            Assert.That(section.FindReward(20), Is.Not.Null);
        }

        [Test]
        public void ChangeRewardPoints()
        {
            var questRepository = new QuestTest.QuestRepository();
            var sectionRepository = new QuestTest.QuestSectionRepository();
            Take10PointReward(questRepository, sectionRepository);
            
            var service = QuestTest.CreateService(questRepository, sectionRepository, QuestTest.CreateQuestConfig(), new ConfigCollection<QuestSectionRewardId, QuestSectionRewardConfig>(
                new []
                {
                    new QuestSectionRewardConfig
                    {
                        RequiredPoints = 15,
                        Reward = new RewardItemConfig
                        {
                            Type = RewardType.Currency,
                            Id = Currency.Soft.ToString(),
                            Count = 1000
                        },
                        Section = QuestSectionType.Daily
                    }
                }));
            var section = service.GetSection(QuestSectionType.Daily);
            Assert.That(section.FindReward(SECTION_POINTS), Is.Null);
            
            var reward = section.FindReward(15);
            Assert.That(reward, Is.Not.Null);
            Assert.That(reward?.RewardTaken, Is.False);
        }

        [Test]
        public void StoreNotTakenRewards()
        {
            var service = QuestTest.CreateService();
            QuestTest.Kill3Enemies(service);
            var quest = service.FindQuest(QuestTest.QUEST_A);
            service.TakeQuestReward(QuestTest.QUEST_A);  //take quest reward to remove it from TakeOldRewards list
            quest.MoveToPast();
            service.RestartTimedOutQuests();
            var rewards = service.TakeOldRewards();
            Assert.That(rewards.Count, Is.EqualTo(1));

            Assert.That(rewards[0].ToRewardItem(), Is.EqualTo(new RewardItem(Currency.Soft.ToString(), RewardType.Currency, 1000)));
        }

        [Test]
        public void ResetRewardStateOnTimeout()
        {
            var service = QuestTest.CreateService();
            QuestTest.Kill3Enemies(service);
            service.TakeSectionReward(new QuestSectionRewardId(QuestSectionType.Daily, SECTION_POINTS));
            
            var quest = service.FindQuest(QuestTest.QUEST_A);
            quest.MoveToPast();
            service.RestartTimedOutQuests();

            var section = service.GetSection(QuestSectionType.Daily);
            var reward = section.FindReward(SECTION_POINTS).Value;
            Assert.That(section.GetRewardState(reward), Is.EqualTo(QuestSection.RewardState.Active));
        }
    }
}