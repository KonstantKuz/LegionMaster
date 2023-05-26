using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Reward.Model;

namespace LegionMaster.Shop.Config
{
    [DataContract]
    public class ProductConfig : ICollectionItem<string>
    {
        [DataMember]
        public string ProductId;
        [DataMember]
        public Currency Currency;
        [DataMember]
        public int CurrencyCount;
        
        [DataMember]
        public string PackId;
        public IEnumerable<RewardItem> GetRewards(PackConfigCollection packConfig) => packConfig.GetRewards(PackId);
        public string Id => ProductId;
    }
}