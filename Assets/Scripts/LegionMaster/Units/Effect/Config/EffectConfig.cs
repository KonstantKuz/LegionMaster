using System.Collections.Generic;
using LegionMaster.Units.Config;

namespace LegionMaster.Units.Effect.Config
{
    public class EffectConfig
    {
        public EffectType Id { get; }
     
        public EffectParamsConfig Params { get; set; }
        
        public IReadOnlyList<ModifierConfig> Modifiers { get; }
        
        public EffectConfig(EffectType id, EffectParamsConfig effectParams, IReadOnlyList<ModifierConfig> modifiers)
        {
            Id = id;
            Params = effectParams;
            Modifiers = modifiers;
        }
        public bool AutoFinish => Params.Time != 0;
        public bool HasModifiers => Modifiers.Count != 0;
    }
}