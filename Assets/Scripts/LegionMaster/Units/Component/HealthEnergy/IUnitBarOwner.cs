using System;

namespace LegionMaster.Units.Component.HealthEnergy
{
    public interface IUnitBarOwner
    {
        public void Init(Unit unit);
        IObservable<float> CurrentValue { get; }
        int MaxValue { get; }
        bool BarEnabled { get; }
    }
}