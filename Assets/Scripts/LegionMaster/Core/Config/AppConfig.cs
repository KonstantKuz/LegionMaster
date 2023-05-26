using System.IO;
using System.Runtime.Serialization;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.Core.Config
{
    public class AppConfig : ILoadableConfig
    {
        private static AppConfig _instance;

        [DataMember(Name = "FactionEnabled")]
        private bool _factionEnabled;
        
        [DataMember(Name = "TutorialEnabled")]
        private bool _tutorialEnabled;

        [DataMember(Name = "HyperCasual")] 
        private bool _hyperCasual;
        
        public static bool FactionEnabled => _instance._factionEnabled;
        public static bool TutorialEnabled => _instance._tutorialEnabled;

        public static bool IsHyperCasual => _instance._hyperCasual;

        public void Load(Stream stream)
        {
            _instance = new CsvSerializer().ReadSingleObject<AppConfig>(stream);
        }
    }
}