using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad.Model
{
    public class UnitButtonModel
    {
        public bool Enabled;
        public bool Interactable;
        public bool CheckMark;
        public string IconPath;
        public Action OnClick;
        public bool ShowNotification;
        public List<Sprite> FactionIcons;
        public string UnitId;
    }

    public static class UnitButtonListModel
    {
        public static Dictionary<string, ReactiveProperty<UnitButtonModel>> BuildButtonModelDictionary(
            IEnumerable<string> unitIds,
            Action<string> onClick,
            Func<string, string> iconPathProvider,
            Func<string, bool> notificationBuilder = null,
            Func<string, List<Sprite>> factionIconLoader = null)
        {
            return unitIds.Select(unitId => (unitId, new ReactiveProperty<UnitButtonModel>(
                new UnitButtonModel
                {
                    IconPath = iconPathProvider.Invoke(unitId),
                    Enabled = true,
                    Interactable = true,
                    CheckMark = false,
                    OnClick = () => onClick?.Invoke(unitId),
                    ShowNotification = notificationBuilder?.Invoke(unitId) ?? false,
                    FactionIcons = factionIconLoader?.Invoke(unitId) ,
                    UnitId = unitId
                }))).ToDictionary(pair => pair.unitId, pair => pair.Item2);
        }        
    }
}