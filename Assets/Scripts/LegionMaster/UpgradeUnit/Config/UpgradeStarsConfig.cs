using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LegionMaster.UpgradeUnit.Config
{
    public class UpgradeStarsConfig
    {
        [DataContract]
        public class ItemConfig
        {
            [DataMember(Name = "Star")]
            public int Star;  
            [DataMember(Name = "Parameters")]
            public UpgradeParametersConfig UpgradeParametersConfig;
        }
        
        private readonly IReadOnlyList<ItemConfig> _items;

        public UpgradeStarsConfig(IReadOnlyList<ItemConfig> items)
        {
            _items = items;
        }

        public IEnumerable<ItemConfig> GetUpgradeStarConfigsByStar(int star)
        {
            return _items.Where(it => it.Star <= star).ToList();
        }
    }
}