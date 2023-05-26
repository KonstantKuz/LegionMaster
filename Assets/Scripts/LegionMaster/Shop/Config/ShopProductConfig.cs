using System.Runtime.Serialization;
using LegionMaster.UI.Screen.Shop;
using LegionMaster.UI.Screen.Shop.Model;

namespace LegionMaster.Shop.Config
{
    [DataContract]
    public class ShopProductConfig
    {
        [DataMember] 
        public string ProductId;
        [DataMember] 
        public ShopItemViewType ViewType;
        [DataMember] 
        public string ViewPrefabId;
        [DataMember]
        public string Bonus;
    }
}