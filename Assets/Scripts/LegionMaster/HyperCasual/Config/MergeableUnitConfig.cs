using System.Runtime.Serialization;
using LegionMaster.Config;

namespace LegionMaster.HyperCasual.Config
{
    [DataContract]
    public class MergeableUnitConfig : ICollectionItem<string>
    {
        [DataMember]
        public int Level;
        [DataMember]
        public string UnitId;   
        [DataMember]
        public float SizeIncreaseFactor;
        public string Id => Level.ToString();
    }
}