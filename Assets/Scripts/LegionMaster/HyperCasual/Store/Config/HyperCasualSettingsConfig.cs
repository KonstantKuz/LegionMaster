using System;
using System.IO;
using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.HyperCasual.Store.Data;

namespace LegionMaster.HyperCasual.Store.Config
{
    public class HyperCasualSettingsConfig : ILoadableConfig
    {
        private HyperCasualSettingsConfig _config;

        [DataMember(Name = "MeleeUnitCost")]
        private int _meleeUnitCost;
        [DataMember(Name = "RangedUnitCost")]
        private int _rangedUnitCost;
        [DataMember(Name = "CostFactor")]
        private float _costFactor;
        [DataMember(Name = "InitialCoinsAmount")]
        private int _initialCoinsAmount;

        public int MeleeUnitCost => _config._meleeUnitCost;

        public int RangedUnitCost => _config._rangedUnitCost;
        public float CostFactor => _config._costFactor;
        public int InitialCoinsAmount => _config._initialCoinsAmount;

        public void Load(Stream stream)
        {
            _config = new CsvSerializer().ReadSingleObject<HyperCasualSettingsConfig>(stream);
        }

        public int GetInitialUnitCost(MergeableUnitType product)
        {
            return product switch {
                    MergeableUnitType.Melee => MeleeUnitCost,
                    MergeableUnitType.Ranged => RangedUnitCost,
                    _ => throw new ArgumentOutOfRangeException(nameof(product), product, null)
            };
        }
    }
}