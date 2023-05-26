using System.Runtime.Serialization;
using LegionMaster.Config;

namespace LegionMaster.UpgradeUnit.Config
{
    [DataContract]
    public class UpgradeLevelConfig : ICollectionItem<string>
    {
        [DataMember(Name = "UnitId")]
        public string UnitId;        
        
        [DataMember(Name = "LevelCostIncrease")]
        public int LevelCostIncrease;  
        
        [DataMember(Name = "Parameters")]
        public UpgradeParametersConfig UpgradeParametersConfig;
        public string Id => UnitId;
    }
}