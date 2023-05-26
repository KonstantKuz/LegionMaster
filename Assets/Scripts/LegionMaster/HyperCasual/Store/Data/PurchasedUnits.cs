using System.Collections.Generic;
using Newtonsoft.Json;

namespace LegionMaster.HyperCasual.Store.Data
{
    public class PurchasedUnits
    {
        [JsonProperty] 
        private Dictionary<MergeableUnitType, int> _purchases;
        
        public PurchasedUnits()
        {
            _purchases = new Dictionary<MergeableUnitType, int>();
        }
        
        public void Increase(MergeableUnitType product)
        {
            if (_purchases.ContainsKey(product)) { 
                ++_purchases[product];
            } else {
                _purchases[product] = 1;
            }
        }
        public int GetCount(MergeableUnitType product) => _purchases.ContainsKey(product) ? _purchases[product] : 0;
    }
}