using System;
using UniRx;

namespace LegionMaster.UI.Screen.Footer.Model
{
    public struct FooterButtonModel
    {
        public IReadOnlyReactiveProperty<FooterButtonType> SelectedButton;
        public Action<FooterButtonType> OnClick;
        public IObservable<bool> Notification;
        public FooterButtonType Type;
    }
}