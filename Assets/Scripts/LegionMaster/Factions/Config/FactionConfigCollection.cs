using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Units.Config;

namespace LegionMaster.Factions.Config
{
    [PublicAPI]
    public class FactionConfigCollection: ILoadableConfig
    {
        private IReadOnlyList<FactionConfig> _factions;
        public void Load(Stream stream)
        {
            _factions = new CsvSerializer().ReadNestedTable<ModifierConfig>(stream)
                .Select(it => new FactionConfig(it.Key, it.Value))
                .ToList();
        }

        public FactionConfig GetFactionConfig(string factionId)
        {
            return _factions.FirstOrDefault(it => it.Id == factionId) ??
                   throw new Exception($"No faction with id {factionId} found in config");
        }
    }
}