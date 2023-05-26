using System;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad.Faction.Model
{
    public class FactionItemModel
    {
        public string IconPath;
        public string FactionUnitCountText;
        public bool Activated;
        public Action<RectTransform> OnClick;
        
        public static FactionItemModel Create(string factionId, string factionUnitCountText, bool activated, Action<string, RectTransform> onClickFaction)
        {
            return new FactionItemModel() {
                    IconPath = Util.IconPath.GetFraction(factionId),
                    FactionUnitCountText = factionUnitCountText,
                    Activated = activated,
                    OnClick = (position) => onClickFaction?.Invoke(factionId, position),
            };
        }
    }
}