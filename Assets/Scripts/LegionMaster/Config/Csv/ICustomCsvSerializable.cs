using System;

namespace LegionMaster.Config.Csv
{
    public interface ICustomCsvSerializable
    {
        void Deserialize(Func<string, string> data);
    }
}