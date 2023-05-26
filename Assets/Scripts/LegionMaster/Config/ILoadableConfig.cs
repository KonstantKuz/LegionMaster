using System.IO;

namespace LegionMaster.Config
{
    public interface ILoadableConfig
    {
        void Load(Stream stream);
    }
}