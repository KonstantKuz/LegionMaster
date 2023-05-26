using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.UpgradeUnit.Config
{
    [PublicAPI]
    public class UpgradeStarsConfigCollection: ILoadableConfig
    {
        [DataMember(Name = "UpgradeUnits")]
        private IReadOnlyDictionary<string, UpgradeStarsConfig> _configs;

        public UpgradeStarsConfig GetConfig(string unitId)
        {
            return _configs[unitId];
        }
        
        public void Load(Stream stream)
        {
            var parsed = new CsvSerializer().ReadNestedTable<UpgradeStarsConfig.ItemConfig>(stream);
            _configs = parsed.ToDictionary(
                it => it.Key, 
                it => new UpgradeStarsConfig(it.Value));
        }
    }
}