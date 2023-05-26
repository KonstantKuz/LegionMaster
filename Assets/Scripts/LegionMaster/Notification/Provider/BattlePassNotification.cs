using System;
using System.Linq;
using LegionMaster.BattlePass.Model;
using LegionMaster.BattlePass.Service;
using UniRx;

namespace LegionMaster.Notification.Provider
{
    public class BattlePassNotification : INotification
    {
        public IObservable<int> NotificationCount { get; }

        private BattlePassNotification(BattlePassService service)
        {
            NotificationCount = service.StateChanged.Select(it => {
                return service.BuildAllRewards().Count(reward => reward.State == BattlePassRewardState.Available);
            });
        }
    }
}