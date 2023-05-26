using System;

namespace LegionMaster.Location.Session.Service
{
    public struct SessionParams
    {
        public Type SessionType;
        public object[] Params;
        public SessionParams(Type sessionType, object[] @params)
        {
            SessionType = sessionType;
            Params = @params;
        }
        public SessionParams(Type sessionType)
        {
            SessionType = sessionType;
            Params = new object[] { };
        }
    }
}