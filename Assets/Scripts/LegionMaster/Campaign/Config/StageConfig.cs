using System.Runtime.Serialization;
namespace LegionMaster.Campaign.Config
{
    [DataContract]
    public class StageConfig
    {
        [DataMember]
        public int Stage;
        [DataMember]
        public int ReinforcedUnitCount;
    }
}