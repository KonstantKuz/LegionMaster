using System.Collections.Generic;
using System.Linq;
using LegionMaster.Analytics;
using LegionMaster.BattlePass.Config;
using LegionMaster.BattlePass.Model;
using LegionMaster.BattlePass.Service;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Repository;
using LegionMaster.Reward.Model;
using LegionMaster.Reward.Service;
using NSubstitute;
using NUnit.Framework;
using SuperMaxim.Messaging;
using Zenject;

namespace Tests.BattlePass
{
    [TestFixture]
    public class BattlePassTest : ZenjectUnitTestFixture
    {
        private const string LEVEL_2_REWARD_ID = "UnitRobotCudgel";      
        private const string LEVEL_3_REWARD_ID = "UnitKatanaNinja";
        private static BattlePassService CreateService(ISingleModelRepository<BattlePassProgress> progressRepository = null,
                                                       ISingleModelRepository<BattlePassRewardCollection> takenRewardsCollection = null,
                                                       BattlePassConfigList battlePassConfigList = null,
                                                       IRewardApplyService rewardApplyService = null)
        {
            return new BattlePassService(progressRepository ?? new BattlePassProgressRepository(), 
                                         battlePassConfigList ?? CreateBattlePassConfig(),
                                         rewardApplyService ?? Substitute.For<IRewardApplyService>(),
                                         takenRewardsCollection ?? new BattlePassRewardRepository(),
                                         Substitute.For<Analytics>(new List<IAnalyticsImpl>()));
        }

        private class BattlePassProgressRepository : SingleModelRepository<BattlePassProgress>
        {
        }

        private class BattlePassRewardRepository : SingleModelRepository<BattlePassRewardCollection>
        {
        }

        private static BattlePassConfigList CreateBattlePassConfig()
        {
            return new BattlePassConfigList(new[] {
                    new BattlePassConfig {
                            Level = 1,
                            ExpToNextLevel = 100,
                            BasicRewardId = Currency.Soft.ToString(),
                            BasicRewardType = RewardType.Currency,
                            BasicRewardCount = 1000,
                            PremiumRewardId = Currency.Soft.ToString(),
                            PremiumRewardType = RewardType.Currency,
                            PremiumRewardCount = 3000,
                    },
                    new BattlePassConfig {
                            Level = 2,
                            ExpToNextLevel = 150,
                            BasicRewardId = LEVEL_2_REWARD_ID,
                            BasicRewardType = RewardType.Shards,
                            BasicRewardCount = 10,
                            PremiumRewardId = LEVEL_2_REWARD_ID,
                            PremiumRewardType = RewardType.Shards,
                            PremiumRewardCount = 50,
                    },
                    new BattlePassConfig {
                            Level = 3,
                            ExpToNextLevel = 200,
                            BasicRewardId = LEVEL_3_REWARD_ID,
                            BasicRewardType = RewardType.Shards,
                            BasicRewardCount = 10,
                            PremiumRewardId = null,
                            PremiumRewardType = RewardType.None,
                            PremiumRewardCount = 0,
                    }
            });
        }

        [Test]
        public void TestIncreaseExp()
        {
            var service = CreateService();

            Assert.That(service.Exp.Value, Is.EqualTo(0));
            Assert.That(service.Level.Value, Is.EqualTo(1));
            Assert.That(service.BattlePassProgress.IsMaxLevel(CreateBattlePassConfig()), Is.False);
            Assert.That(service.BattlePassProgress.MaxExpForCurrentLevel(CreateBattlePassConfig()), Is.EqualTo(100));
            Assert.That(service.BattlePassProgress.GetNeedExpUntilNextLevel(CreateBattlePassConfig()), Is.EqualTo(100));

            service.AddExp(99);
            Assert.That(service.Exp.Value, Is.EqualTo(99));
            Assert.That(service.Level.Value, Is.EqualTo(1));
            Assert.That(service.BattlePassProgress.IsMaxLevel(CreateBattlePassConfig()), Is.False);
            Assert.That(service.BattlePassProgress.MaxExpForCurrentLevel(CreateBattlePassConfig()), Is.EqualTo(100));
            Assert.That(service.BattlePassProgress.GetNeedExpUntilNextLevel(CreateBattlePassConfig()), Is.EqualTo(1));

            service.AddExp(1);
            Assert.That(service.Exp.Value, Is.EqualTo(0));
            Assert.That(service.Level.Value, Is.EqualTo(2));
            Assert.That(service.BattlePassProgress.IsMaxLevel(CreateBattlePassConfig()), Is.False);
            Assert.That(service.BattlePassProgress.MaxExpForCurrentLevel(CreateBattlePassConfig()), Is.EqualTo(150));
            Assert.That(service.BattlePassProgress.GetNeedExpUntilNextLevel(CreateBattlePassConfig()), Is.EqualTo(150));

            service.AddExp(150);
            Assert.That(service.Exp.Value, Is.EqualTo(0));
            Assert.That(service.Level.Value, Is.EqualTo(3));
            Assert.That(service.BattlePassProgress.IsMaxLevel(CreateBattlePassConfig()), Is.True);
            Assert.That(service.BattlePassProgress.MaxExpForCurrentLevel(CreateBattlePassConfig()), Is.EqualTo(200));
            Assert.That(service.BattlePassProgress.GetNeedExpUntilNextLevel(CreateBattlePassConfig()), Is.EqualTo(200));

            service.AddExp(300);
            Assert.That(service.Exp.Value, Is.EqualTo(200));
            Assert.That(service.Level.Value, Is.EqualTo(3));
            Assert.That(service.BattlePassProgress.IsMaxLevel(CreateBattlePassConfig()), Is.True);
            Assert.That(service.BattlePassProgress.MaxExpForCurrentLevel(CreateBattlePassConfig()), Is.EqualTo(200));
            Assert.That(service.BattlePassProgress.GetNeedExpUntilNextLevel(CreateBattlePassConfig()), Is.EqualTo(0));
        }

        [Test]
        public void TestBuildAllRewards()
        {
            var service = CreateService();
            var battlePassRewards = service.BuildAllRewards().ToList();
            Assert.That(battlePassRewards.Count, Is.EqualTo(CreateBattlePassConfig().Items.Count * 2));     
            Assert.That(service.Level.Value, Is.EqualTo(1));

            var rewardId = new BattlePassRewardId(service.Level.Value, BattlePassRewardType.Basic);
            var expectedReward = new BattlePassReward(rewardId, CreateBattlePassConfig().FindRewardById(rewardId), BattlePassRewardState.Available);
            Assert.That(battlePassRewards, Has.Member(expectedReward));

            
            Assert.That(FreeReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Available));  
            Assert.That(FreeReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Unavailable)); 
            Assert.That(FreeReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.Unavailable));       
            Assert.That(PremiumReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Unavailable));   
            Assert.That(PremiumReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Unavailable));     
            Assert.That(PremiumReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.NoReward));
        
            IncreaseLevel(service);
            Assert.That(service.Level.Value, Is.EqualTo(2));
            battlePassRewards = service.BuildAllRewards().ToList();
            
            Assert.That(FreeReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Available));  
            Assert.That(FreeReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Available)); 
            Assert.That(FreeReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.Unavailable));       
            Assert.That(PremiumReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Unavailable));   
            Assert.That(PremiumReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Unavailable));     
            Assert.That(PremiumReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.NoReward));
        }

        [Test]
        public void TestActivatePremium()
        {
            var service = CreateService();
            Assert.That(service.PremiumActive.Value, Is.False);
            
            service.UpdatePremium(true);
            Assert.That(service.PremiumActive.Value, Is.True);
            
            IncreaseLevel(service);
            Assert.That(service.Level.Value, Is.EqualTo(2));     
            
            var battlePassRewards = service.BuildAllRewards().ToList();

            Assert.That(FreeReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Available));  
            Assert.That(FreeReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Available)); 
            Assert.That(FreeReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.Unavailable));       
            Assert.That(PremiumReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Available));   
            Assert.That(PremiumReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Available));     
            Assert.That(PremiumReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.NoReward));
        } 
        [Test]
        public void TestTakeReward()
        {
            var service = CreateService();
            Assert.That(service.Level.Value, Is.EqualTo(1));    
            var rewardId = new BattlePassRewardId(service.Level.Value, BattlePassRewardType.Basic);
            var takenReward = service.TakeReward(rewardId);
            
            
            var expectedReward = new RewardItem(Currency.Soft.ToString(), RewardType.Currency, 1000);
            Assert.That(takenReward, Is.EqualTo(expectedReward));
            
            var battlePassRewards = service.BuildAllRewards().ToList();

            Assert.That(FreeReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Taken));  
            Assert.That(FreeReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Unavailable)); 
            Assert.That(FreeReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.Unavailable));       
            Assert.That(PremiumReward(battlePassRewards, 1).State, Is.EqualTo(BattlePassRewardState.Unavailable));   
            Assert.That(PremiumReward(battlePassRewards, 2).State, Is.EqualTo(BattlePassRewardState.Unavailable));     
            Assert.That(PremiumReward(battlePassRewards, 3).State, Is.EqualTo(BattlePassRewardState.NoReward));
            
        }
        private BattlePassReward FreeReward(List<BattlePassReward> battlePassRewards, int level)
        {
            return battlePassRewards.Find(it => it.Id.Equals(new BattlePassRewardId(level, BattlePassRewardType.Basic)));
        }  
        private BattlePassReward PremiumReward(List<BattlePassReward> battlePassRewards, int level)
        {
            return battlePassRewards.Find(it => it.Id.Equals(new BattlePassRewardId(level, BattlePassRewardType.Premium)));
        }
        private void IncreaseLevel(BattlePassService service)
        {
            service.AddExp(service.GetNeededExpUntilNextLevel);
        }
    }
}