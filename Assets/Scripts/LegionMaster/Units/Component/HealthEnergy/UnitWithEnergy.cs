using System;
using LegionMaster.Units.Model;
using UniRx;
using UnityEngine;

namespace LegionMaster.Units.Component.HealthEnergy
{
    public class UnitWithEnergy : MonoBehaviour, IEnergyBarOwner
    {
        private ReactiveProperty<float> _currentEnergy;
    
        private IUnitEnergyModel _energyConfig;
        
        public void Init(Unit unit)
        {
            _energyConfig = unit.UnitModel.UnitEnergy;
            BarEnabled = unit.UnitModel.AbilityEnabled;
            _currentEnergy = new FloatReactiveProperty(_energyConfig.StartingEnergy);
        }

        public void RecoverEnergy(int amount)
        {
            _currentEnergy.Value = Mathf.Min(_currentEnergy.Value + amount, MaxValue);
        }

        public IObservable<float> CurrentValue => _currentEnergy;
        public int MaxValue => _energyConfig.MaxEnergy;
        
        public bool BarEnabled { get; private set; }
        public bool IsFull => _currentEnergy.Value >= MaxValue;

        public void TakeAll() => _currentEnergy.Value = 0;
    }
}