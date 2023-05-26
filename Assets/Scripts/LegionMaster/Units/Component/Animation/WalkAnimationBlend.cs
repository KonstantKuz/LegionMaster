using LegionMaster.Units.Component.Ai;
using UnityEngine;

namespace LegionMaster.Units.Component.Animation
{
    [RequireComponent(typeof(IMoving))]
    public class WalkAnimationBlend : MonoBehaviour, IUpdatableUnitComponent
    {
        private class MotionValues
        {
            public float VerticalValue;
            public float HorizontalValue;
        }
        
        private MoveAnimationWrapper _animationWrapper;   
        private IMoving _moving;

        private void Awake()
        {
            _animationWrapper = new MoveAnimationWrapper(GetComponentInChildren<Animator>());
            _moving = GetComponent<IMoving>();
        }

        public void UpdateComponent()
        {
            ApplyMotionValues(CalculateMotionValues());
        }

        private void ApplyMotionValues(MotionValues values)
        {
            _animationWrapper.SetMotionValues(values.VerticalValue, values.HorizontalValue);
        }

        private MotionValues CalculateMotionValues()
        {
            if (!_moving.IsMoving) {
                return new MotionValues();
            }

            var targetDirectionAngle = TargetDirectionAngle();

            var verticalSign = Mathf.Abs(targetDirectionAngle) <= 90 ? 1.0f : -1.0f;
            var horizontalSign = Mathf.Sign(targetDirectionAngle);
            
            var angle = Mathf.Abs(targetDirectionAngle);
            angle = angle <= 90 ? angle : 180 - angle;

            return new MotionValues
            {
                VerticalValue = Mathf.InverseLerp(90, 0, angle) * verticalSign,
                HorizontalValue = Mathf.InverseLerp(0, 90, angle) * horizontalSign
            };
        }

        private float TargetDirectionAngle()
        {
            return Vector3.SignedAngle(_moving.LookAtDirection, _moving.MovementDirection, transform.up);
        }
    }
}