using System;
using System.Runtime.Serialization;
using LegionMaster.Location.GridArena.Config;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.Location.Session.Config;
using LegionMaster.Units.Model;

namespace LegionMaster.Enemy.Config
{
    [DataContract]
    public class EnemySpawnConfig
    {
        
        [DataMember(Name = "UnitId")]
        private string _unitId;
        [DataMember(Name = "XFromLeft")]
        private int _x;
        [DataMember(Name = "YFromBottom")]
        private int _y;

        [DataMember]
        private UnitUpgradeParams _upgradeParams;

        [DataMember]
        private UnitOverrideParams _overrideParams;

        public UnitSpawnConfig ToUnitSpawnConfig(ArenaGridConfig gridConfig) =>
                ToUnitSpawnConfig((int) gridConfig.Dimensions.y);
        
        public UnitSpawnConfig ToUnitSpawnConfig(int gridSizeY) =>
                new UnitSpawnConfig {
                        UnitId = _unitId,
                        CellId = new CellId(Math.Abs(-gridSizeY + _y), _x - 1),
                        UpgradeParams = _upgradeParams,
                        OverrideParams = _overrideParams
                };
    }
}