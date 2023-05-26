using System;
using SuperMaxim.Messaging;
using UniRx;

namespace LegionMaster.Extension
{
    public static class MessengerExt
    {
        public static IDisposable MessageAsObservable<T>(this IMessenger messenger, Action<T> onNext)
        {
            messenger.Subscribe(onNext);
            return Disposable.Create(() =>
            {
                messenger.Unsubscribe(onNext);
            });
        }
    }
}