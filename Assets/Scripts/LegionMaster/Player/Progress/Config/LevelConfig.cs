using System.Runtime.Serialization;
using LegionMaster.Config;

namespace LegionMaster.Player.Progress.Config
{
    [DataContract]
    public class LevelConfig : ICollectionItem<string>
    {
        [DataMember(Name = "Level")]
        public string Lvl;
        [DataMember]
        public int ExpToNextLevel;
        public string Id => Lvl;
    }
}