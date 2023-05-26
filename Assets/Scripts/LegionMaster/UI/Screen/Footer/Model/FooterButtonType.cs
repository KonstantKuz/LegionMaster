using System;
using LegionMaster.Notification;

namespace LegionMaster.UI.Screen.Footer.Model
{
    public enum FooterButtonType
    {
        Quest,
        Shop,
        Duel,
        Campaign,
        Army
    }

    public static class FooterButtonTypeExt
    {
        public static NotificationType GetNotificationType(this FooterButtonType value)
        {
            return value switch
            {
                    FooterButtonType.Quest => NotificationType.Quest,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }
}