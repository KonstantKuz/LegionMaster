using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Config;
using LegionMaster.Units.Service;
using LegionMaster.UpgradeUnit.Config;
using UniRx;

namespace LegionMaster.Units.Model.Meta
{
    public class UnitModel : IUnitModel
    {
        private readonly UpgradeLevelConfig _upgradeLevelConfig; 
        private readonly UpgradeStarsConfig _upgradeStarsConfig; 
        [CanBeNull]
        private readonly UnitOverrideParams _overrideParams;
        
        public UnitModel(UnitConfig baseUnit, UpgradeStarsConfig upgradeStarsConfig, UpgradeLevelConfig upgradeLevelConfig, 
                         [NotNull] InventoryUnit inventoryUnit, 
                         [CanBeNull] UnitOverrideParams overrideParams)
        {
            _upgradeLevelConfig = upgradeLevelConfig;
            _upgradeStarsConfig = upgradeStarsConfig;
            _overrideParams = overrideParams;
            BaseUnit = baseUnit;
            InventoryUnit = inventoryUnit;
            RankedUnit = RankedUnitModel.Create(baseUnit.RankConfig, InventoryUnit);
            UnitAttack = new UnitAttackModel(baseUnit.AttackConfig, upgradeStarsConfig, upgradeLevelConfig, RankedUnit);
            UnitHealth = new UnitHealthModel(baseUnit.HealthConfig, upgradeStarsConfig, upgradeLevelConfig, RankedUnit);
            UnitEnergy = new UnitEnergyModel(baseUnit.EnergyConfig);
        }

        public string UnitId => BaseUnit.UnitId;
        public int Power => UnitParameterCalculator.CalculatePower(this);
        public int Armor => UnitParameterCalculator.CalculateAddedArmor(_upgradeStarsConfig, _upgradeLevelConfig, RankedUnit) + BaseUnit.Armor;
        public IReadOnlyReactiveProperty<int> MoveSpeed => new ReactiveProperty<int>(BaseUnit.MoveSpeed);
        public float DodgeChance => BaseUnit.DodgeChance;
        public float GetResistance(AttackType attackType) => BaseUnit.GetResistanceConfig(attackType).Resistance;
        
        [NotNull] public InventoryUnit InventoryUnit { get; }
        public IUnitAttackModel UnitAttack { get; }
        public IUnitHealthModel UnitHealth { get; }
        public RankedUnitModel RankedUnit { get; }
        public IUnitEnergyModel UnitEnergy { get; }

        public UnitConfig BaseUnit { get; }
        
        public IEnumerable<string> Factions => BaseUnit.RankConfig.Fractions;

        public int Star
        {
            get => RankedUnit.Star;
            set => RankedUnit.Star = value;
        }
        public bool AbilityEnabled => _overrideParams != null ? _overrideParams.AbilityEnabled && BaseUnit.AbilityEnabled : BaseUnit.AbilityEnabled;
        public bool HudVisible { get; set; }
        public bool AiEnabled { get; set; }
        public bool StarBarVisible { get; set; }

    }
}