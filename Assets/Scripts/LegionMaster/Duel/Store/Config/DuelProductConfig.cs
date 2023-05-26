using System.Runtime.Serialization;
using LegionMaster.Config;

namespace LegionMaster.Duel.Store.Config
{
    public class DuelProductConfig : ICollectionItem<string>
    {
        [DataMember(Name = "UnitId")]
        public string UnitId;
        [DataMember(Name = "Price")]
        public int Price;
        public string Id => UnitId;
    }
}