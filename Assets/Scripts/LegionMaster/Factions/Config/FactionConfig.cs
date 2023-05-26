using System.Collections.Generic;
using LegionMaster.Units.Config;

namespace LegionMaster.Factions.Config
{
    public class FactionConfig
    {
        public string Id { get; }
        public IReadOnlyList<ModifierConfig> Modifiers { get; }

        public FactionConfig(string id, IReadOnlyList<ModifierConfig> modifiers)
        {
            Id = id;
            Modifiers = modifiers;
        }
    }
}