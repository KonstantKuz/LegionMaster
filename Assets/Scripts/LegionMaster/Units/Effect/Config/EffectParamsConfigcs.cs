
using System.Runtime.Serialization;

namespace LegionMaster.Units.Effect.Config
{
    [DataContract]
    public class EffectParamsConfig
    {
        [DataMember]
        public float Time; 
        [DataMember]
        public float DamagePeriod;
    }
}