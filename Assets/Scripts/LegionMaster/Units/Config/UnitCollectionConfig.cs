using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Location.Session.Config;

namespace LegionMaster.Units.Config
{
    [PublicAPI]
    public class UnitCollectionConfig : ILoadableConfig
    {
        public IReadOnlyList<UnitConfig> _unitConfigs;

        public UnitConfig GetUnitConfig(string unitId)
        {
            return _unitConfigs.FirstOrDefault(u => u.UnitId == unitId)
                   ?? throw new NullReferenceException($"UnitConfig is null for unitId:= {unitId}");
        } 
        public int GetUnitStartingStars(string unitId)
        {
            return GetUnitConfig(unitId).RankConfig.StartingStars;
        }
        
        public List<UnitConfig> GetUnitConfigsBySpawn(IEnumerable<UnitSpawnConfig> spawnConfigs)
        {
            return spawnConfigs.Select(s => GetUnitConfig(s.UnitId)).ToList();
        }
        public IEnumerable<string> AllUnitIds => _unitConfigs.Select(it => it.UnitId);

        public void Load(Stream stream)
        {
            _unitConfigs = new CsvSerializer().ReadObjectArray<UnitConfig>(stream);
        }
    }
}