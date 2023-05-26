using System;
using LegionMaster.Units.Component.HealthEnergy;
using UniRx;

namespace LegionMaster.UI.Screen.Battle.HealthBar
{
    public class UnitBarModel
    {
        public readonly bool Enabled;
        public readonly IObservable<float> Percent;    

        public UnitBarModel(IUnitBarOwner owner)
        {
            Enabled = owner.BarEnabled;
            Percent = owner.CurrentValue.Select(it => 1.0f * it / owner.MaxValue);
        }
    }
}