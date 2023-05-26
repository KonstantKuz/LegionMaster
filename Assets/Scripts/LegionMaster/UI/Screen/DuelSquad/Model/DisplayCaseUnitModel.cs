using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.Model
{
    public class DisplayCaseUnitModel
    {
        private ReactiveProperty<DisplayedUnitState> _state;
        public DisplayCaseUnitId Id { get; }
        public bool CanMerge { get; set;  }
        public Action<GameObject> OnStartDrag { get; }
        public Action<GameObject> OnStopDrag { get; }
        [CanBeNull] public PriceModel PriceModel { get; }
        public IReadOnlyCollection<Sprite> FactionIcons { get; }
        public IReactiveProperty<DisplayedUnitState> State => _state;
        public DisplayCaseUnitModel(DisplayCaseUnitId id, 
                                    bool canMerge,
                                    Action<GameObject> onStartDrag, 
                                    Action<GameObject> onStopDrag,
                                    [CanBeNull] PriceModel price, 
                                    List<Sprite> factionIcons)
        {
            Id = id;
            CanMerge = canMerge;
            OnStartDrag = onStartDrag;
            OnStopDrag = onStopDrag;
            _state = new ReactiveProperty<DisplayedUnitState>(DisplayedUnitState.NotTaken);
            PriceModel = price;
            FactionIcons = factionIcons;
        }
        public void Update(DisplayedUnitState state)
        {
            _state.SetValueAndForceNotify(state);
        }
    }
}