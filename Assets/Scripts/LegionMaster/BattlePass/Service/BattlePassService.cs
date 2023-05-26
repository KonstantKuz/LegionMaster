using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.BattlePass.Config;
using LegionMaster.BattlePass.Model;
using LegionMaster.Repository;
using LegionMaster.Reward.Model;
using LegionMaster.Reward.Service;
using UniRx;
using UnityEngine.Assertions;

namespace LegionMaster.BattlePass.Service
{
    public class BattlePassService
    {
        private readonly ISingleModelRepository<BattlePassProgress> _progressRepository;
        private readonly ISingleModelRepository<BattlePassRewardCollection> _takenRewardsCollection;
        private readonly BattlePassConfigList _battlePassConfigList;
        private readonly IRewardApplyService _rewardApplyService;
        private readonly Analytics.Analytics _analytics;
        
        private IntReactiveProperty _exp;
        private IntReactiveProperty _level;
        private BoolReactiveProperty _premiumActive;
        private ReactiveProperty<Unit> _rewardTaken = new ReactiveProperty<Unit>(new Unit());
        public IReadOnlyReactiveProperty<int> Exp => _exp;
        public IReadOnlyReactiveProperty<int> Level => _level;
        public IReadOnlyReactiveProperty<bool> PremiumActive => _premiumActive;
        
        public IObservable<Unit> StateChanged => Exp.Select(it => new Unit())
                                                    .Merge(Level.Select(it => new Unit()))
                                                    .Merge(PremiumActive.Select(it => new Unit()))
                                                    .Merge(_rewardTaken);
        public int GetNeededExpUntilNextLevel => BattlePassProgress.GetNeedExpUntilNextLevel(_battlePassConfigList);
        public int MaxExpForCurrentLevel => BattlePassProgress.MaxExpForCurrentLevel(_battlePassConfigList);
        public bool IsMaxLevel => BattlePassProgress.IsMaxLevel(_battlePassConfigList);
        public bool HasAnyAvailableRewards => BuildAllRewards().Any(it => it.State == BattlePassRewardState.Available);

        public BattlePassService(ISingleModelRepository<BattlePassProgress> progressRepository,
                                 BattlePassConfigList battlePassConfigList,
                                 IRewardApplyService rewardApplyService,
                                 ISingleModelRepository<BattlePassRewardCollection> takenRewardsCollection,
                                 Analytics.Analytics analytics)
        {
            _progressRepository = progressRepository;
            _battlePassConfigList = battlePassConfigList;
            _rewardApplyService = rewardApplyService;
            _takenRewardsCollection = takenRewardsCollection;
            _analytics = analytics;
            _exp = new IntReactiveProperty(BattlePassProgress.Exp);
            _level = new IntReactiveProperty(BattlePassProgress.Level);
            _premiumActive = new BoolReactiveProperty(BattlePassProgress.PremiumActive);
        }

        public void AddExp(int amount, bool bought = false)
        {
            Assert.IsTrue(amount >= 0, "Added amount of BattlePassExp should be non-negative");
            var progress = BattlePassProgress;
            int previousLevel = progress.Level;
            progress.AddExp(amount, _battlePassConfigList);
            SaveProgress(progress);
            _analytics.ReportResourceGained(CurrencyType.BattlePassExp, amount);
            if (previousLevel < progress.Level) {
                _analytics.ReportBattlePassLevelUp(progress.Level, bought);
            }
        }

        public void UpdatePremium(bool activated)
        {
            var progress = BattlePassProgress;
            progress.PremiumActive = activated;
            _premiumActive.Value = progress.PremiumActive;
            _progressRepository.Set(progress);
            if (activated) {
                _analytics.ReportResourceGained(CurrencyType.BattlePassPremium, 1);  
            }
        
        }

        public RewardItem TakeReward(BattlePassRewardId rewardId)
        {
            if (TakenRewardsCollection.IsRewardTaken(rewardId)) {
                throw new Exception($"RewardItem for {rewardId.ToString()} already taken");
            }
            var reward = _battlePassConfigList.FindRewardById(rewardId) ?? throw new Exception($"RewardItem for {rewardId.ToString()} is null");
            _rewardApplyService.ApplyReward(reward);
            SaveTakenReward(rewardId);
            _rewardTaken.SetValueAndForceNotify(new Unit());
            return reward;
        }

        public IEnumerable<BattlePassReward> BuildAllRewards()
        {
            return _battlePassConfigList.Items.SelectMany(config => new[] {
                    BuildReward(config, BattlePassRewardType.Basic), BuildReward(config, BattlePassRewardType.Premium)
            });
        }

        public BattlePassReward BuildReward(BattlePassConfig config, BattlePassRewardType type)
        {
            var rewardId = new BattlePassRewardId(config.Level, type);
            var reward = config.GetReward(type);
            return new BattlePassReward(rewardId, reward, BuildRewardState(config, rewardId, reward));
        }

        private BattlePassRewardState BuildRewardState(BattlePassConfig config, BattlePassRewardId rewardId, [CanBeNull] RewardItem reward)
        {
            if (reward == null) return BattlePassRewardState.NoReward;
            if (config.Level > BattlePassProgress.Level) return BattlePassRewardState.Unavailable;
            if (TakenRewardsCollection.IsRewardTaken(rewardId)) return BattlePassRewardState.Taken;
            if (rewardId.Type == BattlePassRewardType.Premium && !BattlePassProgress.PremiumActive) return BattlePassRewardState.Unavailable;
            return BattlePassRewardState.Available;
        }

        private void SaveProgress(BattlePassProgress progress)
        {
            _progressRepository.Set(progress);
            _level.Value = progress.Level;
            _exp.Value = progress.Exp;
        }

        private void SaveTakenReward(BattlePassRewardId rewardId)
        {
            var takenRewardCollection = TakenRewardsCollection;
            takenRewardCollection.Add(rewardId);
            _takenRewardsCollection.Set(takenRewardCollection);
        }

        public BattlePassProgress BattlePassProgress => _progressRepository.Get() ?? new BattlePassProgress();
        private BattlePassRewardCollection TakenRewardsCollection => _takenRewardsCollection.Get() ?? new BattlePassRewardCollection();
    }
}