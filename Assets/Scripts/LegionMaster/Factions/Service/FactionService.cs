using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Factions.Config;
using LegionMaster.Modifiers;
using LegionMaster.Units;
using LegionMaster.Units.Component;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.Factions.Service
{
    public class FactionService
    {
        public const int MINIMUM_FACTION_UNIT_COUNT = 2;

        [Inject]
        private FactionConfigCollection _factionConfig;
        
        public void ApplyFactionModifiers(IReadOnlyCollection<Unit> units)
        {
            foreach (var unitType in EnumExt.Values<UnitType>())
            {
                var teamUnits = units.Where(it => it.UnitType == unitType).ToList();
                var activeFactions = GetActiveFactions(teamUnits).ToList();
                Debug.Log($"Active factions bonuses for team {unitType} are: [{string.Join(",", activeFactions)}]");

                ApplyModifiersForFactions(units, activeFactions, unitType);
            }
        }

        private void ApplyModifiersForFactions(IReadOnlyCollection<Unit> units, List<string> activeFactions, UnitType unitType)
        {
            foreach (var faction in activeFactions)
            {
                foreach (var modifierCfg in _factionConfig.GetFactionConfig(faction).Modifiers)
                {
                    var modifier = ModifierFactory.Create(modifierCfg);
                    var targetUnits = GetModifierTargets(units, modifierCfg.Target, unitType, faction);
                    targetUnits.ForEach(unit => unit.AddModifier(modifier));
                }
            }
        }

        private static IEnumerable<Unit> GetModifierTargets(
            IEnumerable<Unit> units, 
            ModifierTarget modifierTarget,
            UnitType unitType,
            string faction)
        {
            return modifierTarget switch
            {
                ModifierTarget.Friends => units.Where(it => it.UnitType == unitType),
                ModifierTarget.Enemies => units.Where(it => it.UnitType != unitType),
                ModifierTarget.Faction => units.Where(it => it.UnitType == unitType && it.UnitModel.Factions.Contains(faction)),
                _ => throw new ArgumentOutOfRangeException(nameof(modifierTarget), modifierTarget, null)
            };
        }

        public static IEnumerable<string> GetActiveFactions(IEnumerable<Unit> units)
        {
            return units
                .SelectMany(it => it.UnitModel.Factions)
                .GroupBy(it => it)
                .Where(group => group.Count() >= MINIMUM_FACTION_UNIT_COUNT)
                .Select(it => it.Key);
        }
    }
}