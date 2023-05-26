using System.IO;
using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.Duel.Config
{
    public class DuelConfig : ILoadableConfig
    {
        private DuelConfig _config;
        
        [DataMember(Name = "DuelWinCount")]
        private int _duelWinCount;  
        
        [DataMember(Name = "TokensStartAmount")]
        private int _tokensStartAmount;
        [DataMember(Name = "TokensPerRoundAmount")]
        private int _tokensPerRoundAmount;
        [DataMember(Name = "DuelShopUpdatePrice")]
        private int _shopUpdatePrice;        
        [DataMember(Name = "SecondsBeforeStartRound")]
        private int _secondsBeforeStartRound;
        
        public int DuelWinCount => _config._duelWinCount;      
        public int TokensStartAmount => _config._tokensStartAmount;
        public int TokensPerRoundAmount => _config._tokensPerRoundAmount;
        public int ShopUpdatePrice => _config._shopUpdatePrice;     
        public int SecondsBeforeStartRound => _config._secondsBeforeStartRound;

        public void Load(Stream stream)
        {
            _config = new CsvSerializer().ReadSingleObject<DuelConfig>(stream);
        }
    }
}