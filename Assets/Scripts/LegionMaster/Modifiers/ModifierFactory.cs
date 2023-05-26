using System;
using LegionMaster.Units.Config;

namespace LegionMaster.Modifiers
{
    public static class ModifierFactory
    {
        public static IModifier Create(ModifierConfig modifierCfg)
        {
            return modifierCfg.Modifier switch
            {
                ModifierType.AddValue => new AddValueModifier(modifierCfg.ParameterName, modifierCfg.Value),
                ModifierType.AddPercent => new AddPercentModifier(modifierCfg.ParameterName, modifierCfg.Value),
                ModifierType.AddAttackType => new AddAttackTypeModifier(modifierCfg.ParameterName),
                ModifierType.StrongDefence => new StrongDefenseModifier(modifierCfg.Value),
                _ => throw new ArgumentOutOfRangeException($"Unsupported modifier type {modifierCfg.Modifier}")
            };
        }
    }
}