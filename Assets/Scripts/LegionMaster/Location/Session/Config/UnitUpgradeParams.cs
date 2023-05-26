using System.Runtime.Serialization;

namespace LegionMaster.Location.Session.Config
{
    [DataContract]
    public class UnitUpgradeParams
    {
        public const int DEFAULT_LEVEL = 1;

        [DataMember]
        public int Level;

        [DataMember]
        public int Star;

        public static UnitUpgradeParams Base => new UnitUpgradeParams
        {
            Level = DEFAULT_LEVEL,
            Star = 0
        };
    }
}