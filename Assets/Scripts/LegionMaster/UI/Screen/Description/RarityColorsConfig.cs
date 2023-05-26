using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Units.Model;
using UnityEngine;

namespace LegionMaster.UI.Screen.Description
{
    [CreateAssetMenu(fileName = "RarityColorsConfig", menuName = "LegionMaster/RarityColorsConfig", order = 0)]
    public class RarityColorsConfig : ScriptableObject
    {
        public List<RarityColor> RarityColors;

        public Color GetColorBy(RarityType rarityType)
        {
            return RarityColors.First(rarity => rarity.RarityType == rarityType).Color;
        }
    }

    [Serializable]
    public class RarityColor
    {
        public RarityType RarityType;
        public Color Color;
    }
}