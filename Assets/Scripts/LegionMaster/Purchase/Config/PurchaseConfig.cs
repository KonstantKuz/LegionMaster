using System.Collections.Generic;
using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Reward.Config.Pack;
using LegionMaster.Reward.Model;
using UnityEngine.Purchasing;

namespace LegionMaster.Purchase.Config
{
    [DataContract]
    public class PurchaseConfig : ICollectionItem<string>
    {
        [DataMember]
        public string ProductId;
        
        [DataMember]
        public string PackId;
        
        [DataMember]
        public string StoreProductId;
        [DataMember]
        public ProductType ProductType;
        public IEnumerable<RewardItem> GetRewards(PackConfigCollection packConfig) => packConfig.GetRewards(PackId);

        public string Id => ProductId;
    }
}