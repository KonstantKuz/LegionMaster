using System;
using UniRx;

namespace LegionMaster.Extension
{
    public static class DisposableExtenstion
    {
        public static void AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
    }
}