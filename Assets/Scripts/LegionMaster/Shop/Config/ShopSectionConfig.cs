using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.UI.Screen.Shop;

namespace LegionMaster.Shop.Config
{
    [DataContract]
    public class ShopSectionConfig
    {
        [DataMember] 
        public string SectionId;
        [DataMember]
        public IReadOnlyList<ShopProductConfig> Products;
    }
}