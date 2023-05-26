using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegionMaster.Tutorial.WaitConditions
{
    public class WaitForClick : CustomYieldInstruction
    {
        private bool _completed;

        public override bool keepWaiting => !_completed;

        public WaitForClick(UIBehaviour gameObject)
        {
            gameObject.OnPointerClickAsObservable().First().Subscribe(it => _completed = true);
        }
    }
}