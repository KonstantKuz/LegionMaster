using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Core.Mode;
using LegionMaster.Factions.Service;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Squad.Model;
using LegionMaster.Player.Squad.Service;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Squad.Faction.Model
{
    public class FactionModel
    {
        private readonly ReactiveProperty<IReadOnlyCollection<FactionItemModel>> _factions;
        public IReadOnlyReactiveProperty<IReadOnlyCollection<FactionItemModel>> Factions => _factions;

        private CompositeDisposable _disposable;

        public FactionModel(PlayerSquadService squadService, InventoryService inventoryService, Action<string, RectTransform> onClickFaction)
        {
            _disposable = new CompositeDisposable();
            _factions = new ReactiveProperty<IReadOnlyCollection<FactionItemModel>>();
            squadService.GetSquad(GameMode.Battle).Subscribe(it => BuildFactions(it, inventoryService, onClickFaction));
        }

        private void BuildFactions(SquadModel squadModel, InventoryService inventoryService, Action<string, RectTransform> onClickFaction)
        {
            var units = squadModel.Units.Select(it => inventoryService.GetUnit(it.UnitId));

            var factions = units
                           .SelectMany(it => it.RankedUnit.Fractions)
                           .GroupBy(it => it)
                           .Select(it => BuildFactionItemModel(it, onClickFaction));

            _factions.SetValueAndForceNotify(new List<FactionItemModel>(factions));
        }

        private FactionItemModel BuildFactionItemModel(IGrouping<string, string> factionGrouping, Action<string, RectTransform> onClickFaction)
        {
            var minFactionUnitCount = FactionService.MINIMUM_FACTION_UNIT_COUNT;
            var currentFactionUnitCount = factionGrouping.Count() > minFactionUnitCount ? minFactionUnitCount : factionGrouping.Count();
            var activated = factionGrouping.Count() >= minFactionUnitCount;
            return FactionItemModel.Create(factionGrouping.Key, $"{currentFactionUnitCount}/{minFactionUnitCount}", activated, onClickFaction);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}