using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.HealthEnergy
{
    [RequireComponent(typeof(UnitWithHealth))]
    [RequireComponent(typeof(UnitWithEnergy))]
    [RequireComponent(typeof(AttackUnit))]
    public class HealthEnergyRestorer : MonoBehaviour, IInitializableComponent, IUpdatableUnitComponent
    {
        private const int HEALTH_RESTORE_PERIOD = 1;
        
        [Inject] private UnitService _unitService;
        
        private AttackUnit _attackUnit;
        private bool _initialized;
        private float _timeLeft = HEALTH_RESTORE_PERIOD;

        private Unit _unit;
        private UnitWithEnergy _unitWithEnergy;
        private UnitWithHealth _unitWithHealth;
        

        private IUnitHealthModel HealthConfig => _unit.UnitModel.UnitHealth;
        private IUnitEnergyModel EnergyConfig => _unit.UnitModel.UnitEnergy;

        private void OnDestroy()
        {
            if (!_initialized) return;
            _unitWithHealth.OnDamageTaken -= RecoverPerHit;
            _unitService.OnUnitDeath -= RecoverPerDeath;
            _attackUnit.OnAttacked -= RecoverPerAttack;
        }
        public void Init(Unit unit)
        {
            _unit = unit;
            _unitWithHealth = GetComponent<UnitWithHealth>();
            _unitWithEnergy = GetComponent<UnitWithEnergy>();
            _attackUnit = GetComponent<AttackUnit>();

            _unitWithHealth.OnDamageTaken += RecoverPerHit;
            _unitService.OnUnitDeath += RecoverPerDeath;
            _attackUnit.OnAttacked += RecoverPerAttack;
            _initialized = true;
        }

        public void UpdateComponent()
        {
            RecoverPerSecond();
        }

        private void RecoverPerSecond()
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft > 0) return;
            _unitWithHealth.RecoverHealth(HealthConfig.RecoveryPerSecond);
            _unitWithEnergy.RecoverEnergy(EnergyConfig.RecoveryPerSecond);
            _timeLeft = HEALTH_RESTORE_PERIOD;
        }

        private void RecoverPerAttack()
        {
            _unitWithHealth.RecoverHealth(HealthConfig.RecoveryPerAttack);
            _unitWithEnergy.RecoverEnergy(EnergyConfig.RecoveryPerAttack);
        }

        private void RecoverPerHit()
        {
            _unitWithHealth.RecoverHealth(HealthConfig.RecoveryPerHit);
            _unitWithEnergy.RecoverEnergy(EnergyConfig.RecoveryPerHit);
        }

        private void RecoverPerDeath(Unit diedUnit)
        {
            if (diedUnit == _unit) return;
            _unitWithHealth.RecoverHealth(HealthConfig.RecoveryPerDeath);
            _unitWithEnergy.RecoverEnergy(EnergyConfig.RecoveryPerDeath);
        }
    }
}