using System.Collections;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.Util.Animation;
using UnityEngine;

namespace LegionMaster.UI.Dialog.LootBox.View
{
    public class LootBoxRewardItemPanel : MonoBehaviour
    {
        private const string OPEN_PARAM_NAME = "Open";
        private const string OPENED_STATE_NAME = "Opened";
        private const string OPEN_QUICKLY_PARAM_NAME = "OpenQuickly";
        private const string OPENED_QUICKLY_STATE_NAME = "OpenedQuickly";

        [SerializeField]
        private RewardItemPanel _rewardItemPanel;

        public void Init(RewardItemModel rewardItemModel)
        {
            _rewardItemPanel.Init(rewardItemModel);
        }
        public IEnumerator PlayOpenAnimation()
        {
            yield return AnimatorTween.Create(gameObject).Trigger(OPEN_PARAM_NAME, OPENED_STATE_NAME);
        }
        public IEnumerator PlayOpenQuicklyAnimation()
        {
            AnimatorTween.Create(gameObject).Trigger(OPEN_QUICKLY_PARAM_NAME, OPENED_QUICKLY_STATE_NAME, null);
            yield return null;
        }
    }
}