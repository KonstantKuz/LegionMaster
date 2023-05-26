using System;
using System.Collections;
using LegionMaster.Extension;
using UnityEngine;

namespace LegionMaster.Util.Animation
{
    public class AnimatorTween
    {
        private AdvancedAnimator _advancedAnimator;
        private readonly GameObject _gameObject;

        private AnimatorTween(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public static AnimatorTween Create(GameObject gameObject)
        {
            return new AnimatorTween(gameObject);
        }

        public void Trigger(string triggerName, string stateName, Action stateCompleteCallback)
        {
            AdvancedAnimator.RegisterStateCompleteHandler(stateName, Completed);
            AdvancedAnimator.SetTrigger(triggerName);
            void Completed()
            {
                AdvancedAnimator.UnregisterStateCompleteHandler(stateName, Completed);
                stateCompleteCallback?.Invoke();
            }
        }

        public IEnumerator Trigger(string triggerName, string stateName)
        {
            bool animCompleted = false;
            Trigger(triggerName, stateName, () => { animCompleted = true; });
            while (!animCompleted) {
                yield return null;
            }
            yield break;
        }
        private AdvancedAnimator AdvancedAnimator => _advancedAnimator ??= _gameObject.GetOrCreateComponent<AdvancedAnimator>();
    }
}