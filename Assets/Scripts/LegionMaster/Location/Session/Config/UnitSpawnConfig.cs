using System.Runtime.Serialization;
using JetBrains.Annotations;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.Units.Model;

namespace LegionMaster.Location.Session.Config
{
    [DataContract]
    public class UnitSpawnConfig
    {
        [DataMember]
        public string UnitId;
        [DataMember]
        public CellId CellId;

        public UnitUpgradeParams UpgradeParams;
        [CanBeNull]
        public UnitOverrideParams OverrideParams;
    }
}