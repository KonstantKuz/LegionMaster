using UnityEngine;

namespace LegionMaster.UI.Screen.DuelDebriefing
{
    [RequireComponent(typeof(Animator))]
    public class ScoreItemView : MonoBehaviour
    {
        private static readonly int AppearAnimation = Animator.StringToHash("ScoreMedalAppear");
        private static readonly int HiddenAnimation = Animator.StringToHash("ScoreMedalHidden");
        private static readonly int ShownAnimation = Animator.StringToHash("ScoreMedalShown");
        
        private Animator _animator;

        public void Init(bool isActive, bool withAnimation)
        {
            if (isActive)
            {
                Animator.Play(withAnimation ? AppearAnimation : ShownAnimation);
            }
            else
            {
                Animator.Play(HiddenAnimation);
            }
        }

        private Animator Animator => _animator ??= GetComponent<Animator>();
    }
}