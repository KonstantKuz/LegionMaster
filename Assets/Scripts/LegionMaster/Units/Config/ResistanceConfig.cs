using System.Runtime.Serialization;
using UnityEngine;

namespace LegionMaster.Units.Config
{
    [DataContract]
    public class ResistanceConfig
    {
        [DataMember(Name = "Resistance")]
        private float _resistance;

        public ResistanceConfig(float resistance)
        {
            _resistance = resistance;
        }

        public float Resistance => Mathf.Clamp(_resistance, UnitConfig.MIN_RANGE_VALUE, UnitConfig.MAX_PERCENT_VALUE);
    }
}