using System.Collections.Generic;
using LegionMaster.Core.IoC;
using UnityEngine;

namespace LegionMaster.Core.Loadable
{
    public class AppLoadableChain : MonoBehaviour
    {
        private readonly Queue<AppLoadable> _chains = new Queue<AppLoadable>();

        public void AddLoadable<T>() where T : AppLoadable
        {
            var loadable = AppContext.Container.Instantiate<T>();
            _chains.Enqueue(loadable);
        }
        public void AddLoadable<T>(IEnumerable<object> extraArgs) where T : AppLoadable
        {
            var loadable = AppContext.Container.Instantiate<T>(extraArgs);
            _chains.Enqueue(loadable);
        }

        public void Next()
        {
            if (_chains.Count == 0) {
                Destroy(this);
                return;
            }
            var loadable = _chains.Dequeue();
            Debug.Log($"AppLoadableChain run loadable= {loadable.GetType().Name}");
            loadable.Run(Next);
        }
    }
}