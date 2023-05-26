using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LegionMaster.LootBox.Config
{
    [DataContract]
    public class LootBoxPlacementConfig
    {
        [DataMember]
        public string PlacementId;
        [DataMember]
        public IReadOnlyList<LootBoxProductConfig> Products;
    }
}