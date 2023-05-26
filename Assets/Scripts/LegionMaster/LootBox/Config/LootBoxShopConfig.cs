using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.LootBox.Config
{
    [PublicAPI]
    public class LootBoxShopConfig : ILoadableConfig
    {
        public IReadOnlyList<LootBoxPlacementConfig> Items { get; private set; }
        
        public LootBoxPlacementConfig GetPlacementConfigById(string placementId)
        {
            return Items.FirstOrDefault(it => it.PlacementId == placementId)
                   ?? throw new NullReferenceException($"LootBoxPlacementConfig is null by placementId, placementId:= {placementId}");
        }
        
        public LootBoxPlacementConfig GetPlacementConfigByProductId(string productId)
        {
            return Items.FirstOrDefault(item => item.Products.Any(product => product.ProductId == productId));
        }

        public void Load(Stream stream)
        {
            Items = new CsvSerializer().ReadNestedTable<LootBoxProductConfig>(stream)
                                       .Select(it => new LootBoxPlacementConfig {
                                               PlacementId = it.Key,
                                               Products = it.Value,
                                       })
                                       .ToList();
        }
    }
}