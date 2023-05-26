using System;

namespace LegionMaster.UI.Screen.Quest
{
    public class SectionButtonModel
    {
        public string Caption;
        public DateTime EndTime;
        public Action Action;
        public bool IsSelected;
        public IObservable<bool> HasNotification;
    }
}