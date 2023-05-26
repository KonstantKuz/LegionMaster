using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LegionMaster.Shop.Data
{
    public class ProductCollectables
    {
        [JsonProperty] 
        private Dictionary<string, int> Collectables { get; } = new Dictionary<string, int>();

        public void Increase(string productId)
        {
            if (Collectables.ContainsKey(productId)) { 
                ++Collectables[productId];
            } else {
                Collectables[productId] = 1;
            }
        }
        public int GetPurchaseCount(string productId) => Collectables.ContainsKey(productId) ? Collectables[productId] : 0;
        public int TotalPurchaseNumber => Collectables.Values.Sum();
    }
}