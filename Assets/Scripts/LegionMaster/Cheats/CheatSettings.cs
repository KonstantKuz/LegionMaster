using System.Runtime.Serialization;

namespace LegionMaster.Cheats
{
    [DataContract]
    public class CheatSettings
    {
        [DataMember]
        public bool ConsoleEnabled;

        [DataMember]
        public bool UnitInfoEnabled;
    }
}