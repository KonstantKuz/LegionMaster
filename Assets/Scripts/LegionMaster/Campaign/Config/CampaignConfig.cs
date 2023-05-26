using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using static System.Int32;

namespace LegionMaster.Campaign.Config
{
    [PublicAPI]
    public class CampaignConfig : ILoadableConfig
    {
        private Dictionary<int, ChapterConfig> _chapters;

        public ChapterConfig GetChapter(int chapter)
        {
            if (!_chapters.ContainsKey(chapter)) {
                throw new KeyNotFoundException($"ChapterConfig is null, chapter:= {chapter}");
            }
            return _chapters[chapter];
        } 
        public StageConfig GetStage(int chapter, int stage)
        {
            return GetChapter(chapter).GetStage(stage);
        }

        public void Load(Stream stream)
        {
            var parsed = new CsvSerializer().ReadNestedTable<StageConfig>(stream);
            _chapters = parsed.ToDictionary(it => Parse(it.Key), it => new ChapterConfig {
                    Chapter = Parse(it.Key),
                    Stages = it.Value
            });
        }
    }
}