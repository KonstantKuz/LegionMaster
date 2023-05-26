using System.Collections.Generic;
using System.IO;
using LegionMaster.Config;
using LegionMaster.Config.Csv;

namespace LegionMaster.Localization.Config
{
    public class LocalizationConfig : ILoadableConfig
    {
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Table { get; private set; }

        public void Load(Stream stream)
        {
            Table = new CsvSerializer().ReadDictionaryTable(stream);
        }
    }
}