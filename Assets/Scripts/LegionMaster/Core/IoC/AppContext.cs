using System;
using Zenject;

namespace LegionMaster.Core.IoC
{
    public class AppContext
    {
        private static DiContainer _container;
        
        public static DiContainer Container
        {
            set
            {
                if (_container != null) {
                    throw new Exception("Container already set");
                }
                _container = value;
            }
            get { return _container; }
        }
        
        public static void Clear()
        {
            if (_container == null) {
                return;
            }
            _container = null;
        }
    }
}