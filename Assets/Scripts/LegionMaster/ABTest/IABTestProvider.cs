using System;

namespace LegionMaster.ABTest
{
    public interface IABTestProvider
    {
        event Action OnLoaded;
        void Load();
    }
}