using System.Runtime.Serialization;
using LegionMaster.Reward.Model;

namespace LegionMaster.Reward.Config
{
    [DataContract]
    public class RewardConfig
    {
        [DataMember(Name = "Id")]
        public string Id; 
        
        [DataMember(Name = "Type")]
        public RewardType Type;    
        
        [DataMember(Name = "MinQuantity")]
        public int MinQuantity;   
        
        [DataMember(Name = "MaxQuantity")]
        public int MaxQuantity;   
        
        [DataMember(Name = "Weight")]
        public int Weight;  
        
        [DataMember(Name = "Rounds")]
        public int Rounds;

        public bool HasRounds => Rounds > 0;
    }
}