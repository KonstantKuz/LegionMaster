using System.Runtime.Serialization;
using UnityEngine;

namespace LegionMaster.Units.Config
{
    [DataContract]
    public class DamageBuffConfig
    {
        [DataMember(Name = "DamageBuff")]
        private float _damageBuff;
        public DamageBuffConfig(float damageBuff)
        {
            _damageBuff = damageBuff;
        }
        
        public float DamageBuff => Mathf.Clamp(_damageBuff, UnitConfig.ZERO_VALUE, UnitConfig.MAX_RANGE_VALUE);
    }
}