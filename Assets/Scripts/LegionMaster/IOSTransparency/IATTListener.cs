using System;

namespace LegionMaster.IOSTransparency
{
    public interface IATTListener
    { 
        event Action OnStatusReceived;
        void Init();
    }
}