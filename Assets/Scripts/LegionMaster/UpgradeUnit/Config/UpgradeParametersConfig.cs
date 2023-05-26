using System.Runtime.Serialization;

namespace LegionMaster.UpgradeUnit.Config
{
    [DataContract]
    public class UpgradeParametersConfig
    {
        [DataMember(Name = "Attack")]
        public int Attack;   
        [DataMember(Name = "Armor")]
        public int Armor;     
        [DataMember(Name = "Health")]
        public int Health;
    }
}