using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Enemy.Service;
using LegionMaster.Extension;
using LegionMaster.HyperCasual.Config;
using LegionMaster.HyperCasual.Store;
using LegionMaster.HyperCasual.Store.Data;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.UI.Screen.BattleMode.Model;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Util;
using UniRx;

namespace LegionMaster.UI.Screen.HyperCasualMode.Model
{
    public class HyperCasualModeScreenModel
    {
        public readonly EnemyLevelModel EnemyLevel;
        private readonly StringKeyedConfigCollection<MergeableUnitConfig> _meleeUnits;
        private readonly StringKeyedConfigCollection<MergeableUnitConfig> _rangedUnits;
        private readonly HyperCasualStoreService _storeService;
        private readonly IReadOnlyCollection<PlacedUnitModel> _placedUnits;       
        private readonly int _playerCellCount;

        private readonly Dictionary<MergeableUnitType, ReactiveProperty<PriceButtonModel>> _priceButtons;
        
        public IObservable<bool> CanStartBattle => 
                _placedUnits.ObserveEveryValueChanged(it => it.Any(unitModel => unitModel.IsPlaced));
        public Dictionary<MergeableUnitType, IObservable<PriceButtonModel>> PriceButtons =>
                _priceButtons.ToDictionary(it => it.Key, it => (IObservable<PriceButtonModel>) it.Value);
        
        private IObservable<bool> GridFull => _placedUnits.ObserveEveryValueChanged(it => 
                                                                                            it.Count(unitModel => unitModel.IsPlaced) == _playerCellCount);

        public HyperCasualModeScreenModel(EnemySquadService enemySquad,
                                          StringKeyedConfigCollection<MergeableUnitConfig> meleeUnit,
                                          StringKeyedConfigCollection<MergeableUnitConfig> rangedUnits,
                                          HyperCasualStoreService storeService,
                                          IReadOnlyCollection<PlacedUnitModel> placedUnits, int playerCellCount)
        {
            EnemyLevel = EnemyLevelModel.CreateForHyperCasual(enemySquad);
            _playerCellCount = playerCellCount;
            _meleeUnits = meleeUnit;
            _rangedUnits = rangedUnits;
            _storeService = storeService;
            _placedUnits = placedUnits;
            
            _priceButtons = EnumExt.Values<MergeableUnitType>()
                                   .Select(type => (type, new ReactiveProperty<PriceButtonModel>(BuildPriceButton(type))))
                                   .ToDictionary(pair => pair.type, pair => pair.Item2);
        }

        public PlacedUnitModel BuildDefaultUnit(MergeableUnitType unitType) =>
                new PlacedUnitModel() {
                        Id = GetMergeableUnitConfig(unitType).First().UnitId,
                };

        public void UpdatePriceButton(MergeableUnitType unitType) => 
            _priceButtons[unitType].SetValueAndForceNotify(BuildPriceButton(unitType));

        private StringKeyedConfigCollection<MergeableUnitConfig> GetMergeableUnitConfig(MergeableUnitType product)
        {
            return product switch {
                    MergeableUnitType.Melee => _meleeUnits,
                    MergeableUnitType.Ranged => _rangedUnits,
                    _ => throw new ArgumentOutOfRangeException(nameof(product), product, null)
            };
        }

        private PriceButtonModel BuildPriceButton(MergeableUnitType unitType)
        {
            var price = _storeService.CalculatePrice(unitType);
            return new PriceButtonModel() {
                    Price = price,
                    PriceLabel = price.ToString(),
                    Enabled = true,
                    CanBuy = GridFull.CombineLatest(_storeService.HasEnoughCurrencyAsObservable(unitType), (gridFull, hasEnoughCurrency) => !gridFull && hasEnoughCurrency),
                    CurrencyIconPath = IconPath.GetCurrency(Currency.Soft.ToString())
            };
        }
    }
}