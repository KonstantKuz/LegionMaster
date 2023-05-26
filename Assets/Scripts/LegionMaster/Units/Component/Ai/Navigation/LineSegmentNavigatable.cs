using System;
using LegionMaster.Extension;
using UniRx;
using UnityEngine;

namespace LegionMaster.Units.Component.Ai.Navigation
{
    public class LineSegmentNavigatable : MonoBehaviour, IUpdatableUnitComponent, INavigatable
    {
        private const float ANGLE_PRECISION = 0.1f;
        private const float DISTANCE_PRECISION = 0.01f;
        
        private Vector3 _destination;
        private float _unitSpeed;
        private float _rotateSpeed;
        private bool _isMoving;
        private CompositeDisposable _disposable;
        public event Action OnDestinationReached;

        public Vector3 Destination => _destination;

        public void Init(IReadOnlyReactiveProperty<float> unitSpeed, float rotateSpeed)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            unitSpeed.Subscribe(value => { _unitSpeed = value; }).AddTo(_disposable);
            _rotateSpeed = rotateSpeed;
        }

        public void Stop()
        {
            _isMoving = false;
        }

        public void GoTo(Vector3 destination)
        {
            _destination = destination;
            _isMoving = true;
        }

        public void UpdateComponent()
        {
            if (!_isMoving) return;
            
            if (Vector3Ext.DistanceXZ(transform.position, _destination) < DISTANCE_PRECISION)
            {
                FinishPath();
                return;
            }            
            
            var currentPos = transform.position;
            var moveDir = (_destination - currentPos).XZ().normalized;
            var moveDirQuaternion = Quaternion.LookRotation(moveDir, Vector3.up);

            if (Quaternion.Angle(transform.rotation, moveDirQuaternion) > ANGLE_PRECISION)
            {
                DoRotation(moveDirQuaternion);
            }
            else
            {
                DoMovement(currentPos, _destination, moveDir);
            }
        }

        private void DoMovement(Vector3 currentPos, Vector3 nextPos, Vector3 moveDir)
        {
            var moveDist = Mathf.Min(_unitSpeed * Time.deltaTime, Vector3Ext.DistanceXZ(currentPos, nextPos));
            transform.position += moveDir * moveDist;
        }

        private void DoRotation(Quaternion moveDirQuaternion)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, moveDirQuaternion, Time.deltaTime * _rotateSpeed);
        }

        private void FinishPath()
        {
            Stop();
            OnDestinationReached?.Invoke();
        }
        
        public void SetActive(bool isActive)
        {
            if (!isActive)
            {
                Stop();
            }
            enabled = isActive;
        }
        private void OnDestroy()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}