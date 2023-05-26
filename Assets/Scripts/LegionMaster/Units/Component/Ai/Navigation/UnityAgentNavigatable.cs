using System;
using LegionMaster.Extension;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace LegionMaster.Units.Component.Ai.Navigation
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnityAgentNavigatable : MonoBehaviour, INavigatable, IUpdatableUnitComponent
    {
        [SerializeField] private float _destinationReachedRadius = 0.1f;
        private NavMeshAgent _agent;
        private CompositeDisposable _disposable;
        public event Action OnDestinationReached;

        public Vector3 Destination => _agent.destination;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Init(IReadOnlyReactiveProperty<float> unitSpeed, float rotateSpeed)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            unitSpeed.Subscribe(value => { _agent.speed = value; }).AddTo(_disposable);
            _agent.angularSpeed = rotateSpeed;
            _agent.enabled = true;
        }

        public void Stop()
        {
            _agent.isStopped = true;
        }

        public void GoTo(Vector3 destination)
        {
            _agent.destination = destination;
            _agent.isStopped = false;
        }

        public void UpdateComponent()
        {
            if (_agent.isStopped || !_agent.enabled) return;
            if (!(Vector3Ext.DistanceXZ(transform.position, _agent.destination) < _destinationReachedRadius)) return;
            OnDestinationReached?.Invoke();
            Stop();
        }

        public void SetActive(bool isActive)
        {
            _agent.enabled = isActive;
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}