using System.Runtime.Serialization;
using LegionMaster.Config;

namespace LegionMaster.Player.Inventory.Config
{
    [DataContract]
    public class ResourceConfig : ICollectionItem<string>
    {
        [DataMember(Name = "Id")]
        public string _id;
        [DataMember]
        public int Value;
        public string Id => _id;
    }
}