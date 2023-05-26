using System.Runtime.Serialization;
using LegionMaster.Config;

namespace LegionMaster.Duel.Store.Config
{
    public class UnitRarityConfig : ICollectionItem<string>
    {
        [DataMember(Name = "UnitId")]
        public string UnitId;
        [DataMember(Name = "RarityWeight")]
        public int RarityWeight;
        public string Id => UnitId;
    }
}