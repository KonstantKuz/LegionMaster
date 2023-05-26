using System.Runtime.Serialization;
using LegionMaster.Config;

namespace LegionMaster.Player.PlayerInit.Config
{
    [DataContract]
    public class StartingUnitConfig : ICollectionItem<string>
    {
        [DataMember(Name = "UnitId")]
        public string UnitId;
        [DataMember(Name = "Star")]
        public int Star;
        [DataMember(Name = "Level")]
        public int Level;
        public string Id => UnitId;
    }
}