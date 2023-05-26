using System.Runtime.Serialization;
using LegionMaster.Reward.Model;

namespace LegionMaster.Reward.Config
{
    [DataContract]
    public class RewardBattleConfig
    {
        [DataMember(Name = "Id")]
        public string Id; 
        
        [DataMember(Name = "Type")]
        public RewardType Type;    
        
        [DataMember(Name = "MinNumber")]
        public int MinNumber;   
        
        [DataMember(Name = "MaxNumber")]
        public int MaxNumber;   
        
        [DataMember(Name = "CoEnemiesKilled")]
        public float CoEnemiesKilled;  
        
        [DataMember(Name = "FactorBattleResult")]
        public float FactorBattleResult;
        
    }
}