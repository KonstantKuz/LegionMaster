using System;

namespace LegionMaster.Units.Component.Hud
{
    public interface IStarBarOwner
    {
        IObservable<int> Stars { get; }
        bool StarBarEnabled { get; }
    }
}