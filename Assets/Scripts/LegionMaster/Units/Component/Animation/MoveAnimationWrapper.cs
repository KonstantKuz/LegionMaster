using UnityEngine;

namespace LegionMaster.Units.Component.Animation
{
    public class MoveAnimationWrapper
    {
        private readonly int _verticalMotionHash = Animator.StringToHash("VerticalMotion");
        private readonly int _horizontalMotionHash = Animator.StringToHash("HorizontalMotion");
        private readonly int _motionAnimationHash = Animator.StringToHash("Motion");
        private readonly Animator _animator;

        public MoveAnimationWrapper(Animator animator)
        {
            _animator = animator;
        }

        public void SetMotionValues(float vertical, float horizontal)
        {
            _animator.SetFloat(_verticalMotionHash, vertical);
            _animator.SetFloat(_horizontalMotionHash, horizontal);
        }

        public void PlayMoveForward()
        {
            _animator.Play(_motionAnimationHash);
            SetMotionValues(1, 0);
        }

        public void PlayIdle()
        {
            _animator.Play(_motionAnimationHash);
            SetMotionValues(0, 0);
        }
    }
}