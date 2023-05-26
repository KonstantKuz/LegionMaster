using System.IO;
using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.Player.Squad.Config
{
    public class PlayerSquadConfig : ILoadableConfig
    {
        private PlayerSquadConfig _config;
        
        [DataMember(Name = "MaxPlacedUnitCount")]
        private int _maxPlacedUnitCount;
        public int MaxPlacedUnitCount => _config._maxPlacedUnitCount;

        public void Load(Stream stream)
        {
            _config = new CsvSerializer().ReadSingleObject<PlayerSquadConfig>(stream);
        }
    }
}