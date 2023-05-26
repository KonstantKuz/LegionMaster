using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.UI.Screen.Shop.Model;

namespace LegionMaster.Shop.Config
{
    [PublicAPI]
    public class ShopScreenConfig : ILoadableConfig
    {
        public IReadOnlyList<ShopSectionConfig> SectionConfigs;

        public void Load(Stream stream)
        {
            SectionConfigs = new CsvSerializer().ReadNestedTable<ShopProductConfig>(stream)
                                                .Select(it => new ShopSectionConfig {
                                                        SectionId = it.Key,
                                                        Products = it.Value,
                                                })
                                                .ToList();
        }

        public IEnumerable<string> GetProductIds(ShopSectionId sectionId)
        {
            return SectionConfigs.First(it => it.SectionId == sectionId.ToString()).Products.Select(it => it.ProductId);
        }
    }
}