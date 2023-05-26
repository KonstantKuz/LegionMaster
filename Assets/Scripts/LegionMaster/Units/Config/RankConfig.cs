using System.Runtime.Serialization;
using LegionMaster.Units.Model;
using UnityEngine;

namespace LegionMaster.Units.Config
{
    [DataContract]
    public class RankConfig
    {
        public const int MIN_STAR_VALUE = 0;
        public const int MAX_STAR_VALUE = 7;
        public const int MIN_LEVEL_VALUE = 1;
        public const int MAX_LEVEL_VALUE = 100;

        [DataMember(Name = "Rarity")]
        private RarityType _rarityType;
        
        [DataMember(Name = "StartingStars")]
        private int _startingStars;
        
        [DataMember(Name = "Fraction")]
        private string[] _fractions;

        [DataMember(Name = "Set")]
        private int _set;
        
        public RarityType RarityType => _rarityType;
        public int StartingStars => Mathf.Clamp(_startingStars, MIN_STAR_VALUE, MAX_STAR_VALUE);
        public string[] Fractions => _fractions;
        public int Set => Mathf.Clamp(_set, MIN_LEVEL_VALUE, MAX_LEVEL_VALUE);
    }
}