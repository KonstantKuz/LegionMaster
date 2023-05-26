using System.Runtime.Serialization;

namespace LegionMaster.LootBox.Config
{
    [DataContract]
    public class LootBoxProductConfig
    {
        [DataMember]
        public string ProductId;
    }
}