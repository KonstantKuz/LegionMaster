using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using LegionMaster.Config;
using LegionMaster.Config.Csv;
using UnityEngine.Assertions;

namespace LegionMaster.ProgressUnit.Config
{
    [PublicAPI]
    public class UnitUnlockConfig: ILoadableConfig
    {
        [DataContract]
        private class Config
        {
            [DataMember] 
            public int ShardsPerStars;
        }

        private IReadOnlyList<Config> _configs;

        public int GetPrice(int toStars)
        {
            Assert.IsTrue(toStars >= 0);
            var sum = 0;
            for (var i = 0; i < _configs.Count && i <= toStars; i++)
            {
                sum += _configs[i].ShardsPerStars;
            }
            return sum;
        }

        public void Load(Stream stream)
        {
            _configs = new CsvSerializer().ReadObjectArray<Config>(stream);
        }
    }
}