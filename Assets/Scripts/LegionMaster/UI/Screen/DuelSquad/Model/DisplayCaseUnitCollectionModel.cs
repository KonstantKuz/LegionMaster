using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Duel.Store;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Units.Config;
using LegionMaster.Util;
using ModestTree;
using SuperMaxim.Core.Extensions;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.Model
{
    public class DisplayCaseUnitCollectionModel
    {
        public readonly IReadOnlyCollection<DisplayCaseUnitModel> Units;

        public DisplayCaseUnitCollectionModel(IEnumerable<DisplayCaseUnitModel> units)
        {
            Units = units.ToList();
        }

        public static DisplayCaseUnitCollectionModel CreateForDuel(IEnumerable<string> unitIds,
                                                                   IReadOnlyCollection<PlacedUnitModel> placedUnits,
                                                                   UnitCollectionConfig unitCollectionConfig,
                                                                   WalletService walletService,
                                                                   DuelUnitStoreService duelUnitStore,
                                                                   Action<DisplayCaseUnitId, GameObject> onStartDrag,
                                                                   Action<DisplayCaseUnitId, GameObject> onStopDrag)
        {
            return new DisplayCaseUnitCollectionModel(unitIds.Select((unitId, index) => {
                var id = new DisplayCaseUnitId(unitId, index);
                return new DisplayCaseUnitModel(id, CanMerge(placedUnits, id), 
                                                (item) => onStartDrag?.Invoke(id, item),
                                                (item) => onStopDrag?.Invoke(id, item), 
                                                CreatePrice(unitId, walletService, duelUnitStore),
                                                LoadUnitFactionIcons(unitId, unitCollectionConfig));
            }));
        }      
        public static DisplayCaseUnitCollectionModel CreateForCampaign(IEnumerable<string> unitIds,
                                                                       IReadOnlyCollection<PlacedUnitModel> placedUnits,
                                                                       UnitCollectionConfig unitCollectionConfig,
                                                                       Action<DisplayCaseUnitId, GameObject> onStartDrag,
                                                                       Action<DisplayCaseUnitId, GameObject> onStopDrag)
        {
            return new DisplayCaseUnitCollectionModel(unitIds.Select((unitId, index) => {
                var id = new DisplayCaseUnitId(unitId, index);
                return new DisplayCaseUnitModel(id, CanMerge(placedUnits, id), 
                                                (item) => onStartDrag?.Invoke(id, item),
                                                (item) => onStopDrag?.Invoke(id, item),
                                                null,
                                                LoadUnitFactionIcons(unitId, unitCollectionConfig));
            }));
        }
         
        public DisplayCaseUnitModel GetUnit(DisplayCaseUnitId id) => Units.First(it => it.Id.Equals(id));
        public IEnumerable<string> GetUnplacedOnGridUnitIds() => GetUnplacedOnGridUnit().Select(it => it.UnitId);
        public IEnumerable<DisplayCaseUnitId> GetUnplacedOnGridUnit() => Units.Where(it => it.State.Value != DisplayedUnitState.PlacedOnGrid).Select(it => it.Id);

        private static PriceModel CreatePrice(string unitId, WalletService walletService, DuelUnitStoreService duelUnitStore) =>
                PriceModel.CreateForUnit(unitId, walletService, duelUnitStore);

        private static List<Sprite> LoadUnitFactionIcons(string unitId, UnitCollectionConfig unitCollectionConfig)
        {
            var unitCfg = unitCollectionConfig.GetUnitConfig(unitId);
            return unitCfg.RankConfig.Fractions.Select(IconPath.GetFraction).Select(Resources.Load<Sprite>).ToList();
        }
        public void UpdateMergeStatuses(IReadOnlyCollection<PlacedUnitModel> placedUnits)
        {
            Units.ForEach(it => { it.CanMerge = CanMerge(placedUnits, it.Id); });
        }
        private static bool CanMerge(IEnumerable<PlacedUnitModel> placedUnits, DisplayCaseUnitId displayedUnit)
        {
            return !PlacedUnitModel.GetMergeableUnits(placedUnits, displayedUnit.UnitId, displayedUnit.Star).IsEmpty();
        }
    }
}