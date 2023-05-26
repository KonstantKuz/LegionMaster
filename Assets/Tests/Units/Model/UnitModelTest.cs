using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Location.Session.Config;
using LegionMaster.Modifiers;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model.Battle;
using LegionMaster.Units.Model.Meta;
using LegionMaster.UpgradeUnit.Config;
using NUnit.Framework;

namespace Tests.Units.Model
{

    public class UnitModelTest
    {
        private static readonly AttackConfig AttackCfg = new AttackConfig(10, 1, 1, 0, 0, 
            new [] { AttackType.Physical }, 
            CreateDamageBuffConfig()
             );

        private static Dictionary<AttackType, DamageBuffConfig> CreateDamageBuffConfig()
        {
            return EnumExt.Values<AttackType>().ToDictionary(it => it, it => new DamageBuffConfig(0));
        }

        private static readonly UnitConfig UnitCfg = new UnitConfig(
            "", 
            1, 
            new HealthConfig(), 
            0, 
            0, 
            CreateResistanceConfig(),
            AttackCfg, 
            new RankConfig(),
            new EnergyConfig());

        private static Dictionary<AttackType, ResistanceConfig> CreateResistanceConfig()
        {
            return EnumExt.Values<AttackType>().ToDictionary(it => it, it => new ResistanceConfig(0));
        }

        private static readonly UpgradeLevelConfig UpgradeLevelCfg = new UpgradeLevelConfig()
        {
            UpgradeParametersConfig = new UpgradeParametersConfig()
            {
                Armor = 2,
                Attack = 3, 
                Health = 5
            }
        };
        private static readonly UpgradeStarsConfig UpgradeStarsCfg = new UpgradeStarsConfig(new[] {
                new UpgradeStarsConfig.ItemConfig() {
                        Star = 0, 
                        UpgradeParametersConfig = new UpgradeParametersConfig {
                                Armor = 0, 
                                Attack = 0, 
                                Health = 0
                                        
                        }
                                
                }, 
                new UpgradeStarsConfig.ItemConfig() {
                        Star = 1, 
                        UpgradeParametersConfig = new UpgradeParametersConfig {
                                Armor = 1, 
                                Attack = 1, 
                                Health = 1
                                        
                        }
                                
                },
                new UpgradeStarsConfig.ItemConfig() {
                        Star = 2, 
                        UpgradeParametersConfig = new UpgradeParametersConfig {
                                Armor = 2, 
                                Attack = 2, 
                                Health = 2
                                        
                        }
                                
                }});
        private static readonly UnitUpgradeParams UnitUpgradeLevelParams = new UnitUpgradeParams
        {
                Level = 3, 
                Star = RankConfig.MIN_STAR_VALUE,
        };  
        private static readonly UnitUpgradeParams UnitUpgradeStarParams = new UnitUpgradeParams
        {
                Level = RankConfig.MIN_LEVEL_VALUE, 
                Star = 2,
        };
        
        [Test]
        public void Levels()
        {
            var model = new UnitModel(UnitCfg, UpgradeStarsCfg, UpgradeLevelCfg, InventoryUnit.FromUpgradeParams("", UnitUpgradeLevelParams), null);
            Assert.That(model.RankedUnit.Level, Is.EqualTo(UnitUpgradeLevelParams.Level));
            Assert.That(model.UnitHealth.StartingHealth, Is.EqualTo(UnitCfg.HealthConfig.Health + (UnitUpgradeLevelParams.Level - 1) * UpgradeLevelCfg.UpgradeParametersConfig.Health));
            Assert.That(model.UnitAttack.Attack, Is.EqualTo(UnitCfg.AttackConfig.Attack + (UnitUpgradeLevelParams.Level - 1) * UpgradeLevelCfg.UpgradeParametersConfig.Attack));
            Assert.That(model.Armor, Is.EqualTo(UnitCfg.Armor + (UnitUpgradeLevelParams.Level - 1) * UpgradeLevelCfg.UpgradeParametersConfig.Armor));
        }   
        [Test]
        public void Stars()
        {
            var model = new UnitModel(UnitCfg, UpgradeStarsCfg, UpgradeLevelCfg, InventoryUnit.FromUpgradeParams("", UnitUpgradeStarParams), null);
            Assert.That(model.RankedUnit.Star, Is.EqualTo(UnitUpgradeStarParams.Star));
            Assert.That(model.UnitHealth.StartingHealth, Is.EqualTo(UnitCfg.HealthConfig.Health + UpgradeStarsCfg.GetUpgradeStarConfigsByStar(UnitUpgradeStarParams.Star).Sum(it => it.UpgradeParametersConfig.Health)));
            Assert.That(model.UnitAttack.Attack, Is.EqualTo(UnitCfg.AttackConfig.Attack + UpgradeStarsCfg.GetUpgradeStarConfigsByStar(UnitUpgradeStarParams.Star).Sum(it => it.UpgradeParametersConfig.Attack)));
            Assert.That(model.Armor, Is.EqualTo(UnitCfg.Armor + UpgradeStarsCfg.GetUpgradeStarConfigsByStar(UnitUpgradeStarParams.Star).Sum(it => it.UpgradeParametersConfig.Armor)));
        }

        [Test]
        public void ModifierArmor()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitBattleModel.ARMOR_PARAMETER, 3));
            Assert.That(model.Armor, Is.EqualTo(UnitCfg.Armor + 3));
        }

        private static UnitBattleModel CreateModel()
        {
            return new UnitBattleModel(new UnitModel(UnitCfg, UpgradeStarsCfg, UpgradeLevelCfg, InventoryUnit.FromConfig(UnitCfg), null));
        }

        [Test]
        public void ModifierStartingHealth()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitHealthBattleModel.STARTING_HEALTH_PARAMETER, 3));
            Assert.That(model.UnitHealth.StartingHealth, Is.EqualTo(UnitCfg.HealthConfig.Health + 3));
        }
        
        [Test]
        public void ModifierMaxHealth()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitHealthBattleModel.MAX_HEALTH_PARAMETER, 3));
            Assert.That(model.UnitHealth.MaxHealth, Is.EqualTo(UnitCfg.HealthConfig.Health + 3));
        }        
        
        [Test]
        public void ModifierMoveSpeed()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitBattleModel.MOVE_SPEED_PARAMETER, 3));
            Assert.That(model.MoveSpeed.Value, Is.EqualTo(UnitCfg.MoveSpeed + 3));
        }
        
        [Test]
        public void ModifierDodgeChange()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitBattleModel.DODGE_CHANGE_PARAMETER, 3));
            Assert.That(model.DodgeChance, Is.EqualTo(UnitCfg.DodgeChance + 3));
        }
        
        [Test]
        public void ModifierAttackRange()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitAttackBattleModel.ATTACK_RANGE_PARAMETER, 3));
            Assert.That(model.UnitAttack.AttackRangeInCells, Is.EqualTo(UnitCfg.AttackConfig.AttackRangeInCells + 3));
        }
        
        [Test]
        public void ModifierAttackInterval()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitAttackBattleModel.ATTACK_INTERVAL_PARAMETER, 3));
            Assert.That(model.UnitAttack.AttackInterval, Is.EqualTo(UnitCfg.AttackConfig.AttackInterval + 3));
        }
        
        [Test]
        public void ModifierAttack()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitAttackBattleModel.ATTACK_PARAMETER, 3));
            Assert.That(model.UnitAttack.Attack, Is.EqualTo(UnitCfg.AttackConfig.Attack + 3));
        }
        
        [Test]
        public void ModifierCriticalChance()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitAttackBattleModel.CRITICAL_CHANCE_PARAMETER, 3));
            Assert.That(model.UnitAttack.CriticalChance, Is.EqualTo(UnitCfg.AttackConfig.CriticalChance + 3));
        }
        
        [Test]
        public void ModifierCriticalDamage()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitAttackBattleModel.CRITICAL_DAMAGE_PARAMETER, 3));
            Assert.That(model.UnitAttack.CriticalDamage, Is.EqualTo(UnitCfg.AttackConfig.CriticalDamage + 3));
        }
        
        [Test]
        public void ModifierResistance()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier("FireResistance", 3));
            Assert.That(model.GetResistance(AttackType.Fire), Is.EqualTo(UnitCfg.GetResistanceConfig(AttackType.Fire).Resistance + 3));
        }
        
        [Test]
        public void ModifierDamageBuff()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier("PoisonDamageBuff", 3));
            Assert.That(model.UnitAttack.GetDamageBuff(AttackType.Poison), Is.EqualTo(UnitCfg.AttackConfig.GetDamageBuffConfig(AttackType.Poison).DamageBuff + 3));
        }

        [Test]
        public void RemoveModifier()
        {
            var model = CreateModel();
            var modifier = new AddValueModifier(UnitHealthBattleModel.STARTING_HEALTH_PARAMETER, 3);
            model.AddModifier(modifier);
            model.RemoveModifier(modifier);
            Assert.That(model.UnitHealth.StartingHealth, Is.EqualTo(UnitCfg.HealthConfig.Health));
        }

        [Test]
        public void AddAttackType()
        {
            var model = CreateModel();
            var modifier = new AddAttackTypeModifier("Poison");
            model.AddModifier(modifier);
            Assert.That(model.UnitAttack.AttackTypes, Has.Some.EqualTo(AttackType.Poison));
        }

        [Test]
        public void StrongDefense()
        {
            var model = CreateModel();
            var modifier = new StrongDefenseModifier(50);
            model.AddModifier(modifier);
            Assert.That(model.UnitAttack.Attack, Is.EqualTo(0.5f * AttackCfg.Attack));
            Assert.That(model.UnitHealth.StartingHealth, Is.EqualTo(UnitCfg.HealthConfig.Health + 0.5f * AttackCfg.Attack));
        }

        [Test]
        public void MinimumValue()
        {
            var model = CreateModel();
            model.AddModifier(new AddValueModifier(UnitAttackBattleModel.ATTACK_PARAMETER, -100));
            Assert.That(model.UnitAttack.Attack, Is.EqualTo(0));
        }
    }
}