using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LegionMaster.Campaign.Config
{
    [DataContract]
    public class ChapterConfig
    {
        [DataMember]
        public int Chapter;
        [DataMember]
        public IReadOnlyList<StageConfig> Stages;
        
        public StageConfig GetStage(int stage)
        {
            return Stages.FirstOrDefault(it => it.Stage == stage) ?? throw new NullReferenceException($"StageConfig is null, stage:= {stage}");
        }
    }
}