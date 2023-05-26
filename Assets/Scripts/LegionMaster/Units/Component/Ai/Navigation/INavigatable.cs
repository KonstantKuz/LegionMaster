using System;
using UniRx;
using UnityEngine;

namespace LegionMaster.Units.Component.Ai.Navigation
{
    public interface INavigatable
    {
        public event Action OnDestinationReached;
        public Vector3 Destination { get; }
        
        void Init(IReadOnlyReactiveProperty<float> unitSpeed, float rotateSpeed);
        void Stop();
        void GoTo(Vector3 destination);
        void SetActive(bool isActive);
    }
}