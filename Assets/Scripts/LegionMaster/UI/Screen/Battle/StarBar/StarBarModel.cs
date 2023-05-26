using System;
using LegionMaster.Units.Component.Hud;

namespace LegionMaster.UI.Screen.Battle.StarBar
{
    public class StarBarModel
    {
        public readonly bool Enabled;
        public readonly IObservable<int> Stars;

        public StarBarModel(IStarBarOwner owner)
        {
            Stars = owner.Stars;
            Enabled = owner.StarBarEnabled;
        }
    }
}