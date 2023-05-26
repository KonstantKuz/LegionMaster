using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using LegionMaster.Extension;
using LegionMaster.Units.Config;

namespace LegionMaster.Units.Effect.Config
{
    [PublicAPI]
    public class EffectConfigCollection : ILoadableConfig
    {
        private Dictionary<EffectType, EffectConfig> _effects;

        public void Load(Stream stream)
        {
            _effects = new CsvSerializer().ReadObjectAndNestedTable<EffectParamsConfig, ModifierConfig>(stream)
                                          .ToDictionary(it => EnumExt.ValueOf<EffectType>(it.Key),
                                                        it => CreateEffectConfig(EnumExt.ValueOf<EffectType>(it.Key), it.Value.Item1,
                                                                                 it.Value.Item2));
        } 
        public EffectConfig Get(EffectType effectType)
        {
            return _effects.ContainsKey(effectType) ? _effects[effectType] : throw new NullReferenceException($"EffectConfig not found, effectType:= {effectType}");
        }
        private EffectConfig CreateEffectConfig(EffectType id, EffectParamsConfig effectParams, IReadOnlyList<ModifierConfig> modifiers)
        {
            return new EffectConfig(id, effectParams, modifiers);
        }
    }
}