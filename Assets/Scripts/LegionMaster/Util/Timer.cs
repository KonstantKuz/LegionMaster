using System;
using UniRx;

namespace LegionMaster.Util
{
    public class Timer : IDisposable
    {
        private DateTime _dueTime;
        private IDisposable _disposable;

        public Timer(DateTime dueTime, Action callBack)
        {
            _dueTime = dueTime;
            _disposable = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Subscribe(_ => UpdateTimer(callBack));
        }
        
        public static IDisposable Create(DateTime dueTime, Action callBack)
        {
            return new Timer(dueTime, callBack);
        }
        private void UpdateTimer(Action callBack)
        {
            var timeLeft = _dueTime - DateTime.Now;
            if (timeLeft >= TimeSpan.Zero) {
                return;
            }
            callBack?.Invoke();
            Dispose();
        }
        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}