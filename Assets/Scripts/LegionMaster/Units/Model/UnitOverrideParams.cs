using System.Runtime.Serialization;

namespace LegionMaster.Units.Model
{
    [DataContract]
    public class UnitOverrideParams
    {
        [DataMember]
        public bool AbilityEnabled;
    }
}