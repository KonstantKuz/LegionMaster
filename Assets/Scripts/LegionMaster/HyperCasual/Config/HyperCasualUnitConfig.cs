using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using Zenject;

namespace LegionMaster.HyperCasual.Config
{
    public class HyperCasualUnitConfig
    {
        [Inject(Id = Configs.MERGEABLE_MELEE_UNITS, Optional = true)]
        private StringKeyedConfigCollection<MergeableUnitConfig> _meleeUnitsConfig;
        [Inject(Id = Configs.MERGEABLE_RANGED_UNITS, Optional = true)]
        private StringKeyedConfigCollection<MergeableUnitConfig> _rangedUnitsConfig;
        
        public StringKeyedConfigCollection<MergeableUnitConfig> MeleeUnitsConfig => _meleeUnitsConfig;
        public StringKeyedConfigCollection<MergeableUnitConfig> RangedUnitsConfig => _rangedUnitsConfig;
        
        public int GetMergeLevelOf(string unitId)
        { 
            return GetMergeableUnitConfig(unitId).Level;
        }
        
        public int GetMaxMergeLevelFor(string unitId)
        {
            return UnitsGroupFor(unitId).Count;
        }

        public MergeableUnitConfig GetMergeableUnitConfig(string unitId)
        {
            return UnitsGroupFor(unitId).Find(it => it.UnitId == unitId);
        }
        public List<MergeableUnitConfig> UnitsGroupFor(string unitId)
        {
            return IsMeleeUnit(unitId)
                           ? MeleeUnitsConfig.Values
                           : RangedUnitsConfig.Values;
        }
        public bool IsMeleeUnit(string unitId)
        {
            return _meleeUnitsConfig.Values.FirstOrDefault(it => it.UnitId == unitId) != null;
        }
        public float GetScaleIncrement(string unitId)
        {
            var mergeableUnitConfig = GetMergeableUnitConfig(unitId);
            return mergeableUnitConfig.SizeIncreaseFactor * mergeableUnitConfig.Level;
        }
    }
}