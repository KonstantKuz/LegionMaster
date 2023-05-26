using System;
using System.Collections;
using LegionMaster.LootBox.Model;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog.LootBox.Model;
using LegionMaster.Util.Animation;
using UnityEngine;

namespace LegionMaster.UI.Dialog.LootBox
{
    public class LootBoxOpeningDialogPresenter : BaseDialog, IUiInitializable<LootBoxOpeningInitModel>
    {
        private const string OPEN_COMMON_PARAM_NAME = "OpenCommon";
        private const string OPEN_RARE_PARAM_NAME = "OpenRare";

        [SerializeField]
        private float _openChestTimeout = 1;

        public void Init(LootBoxOpeningInitModel initModel)
        {
            gameObject.SetActive(true);
            StartCoroutine(OpenChest(initModel.OnAnimationComplete, GetAnimationParam(initModel.LootBoxId)));
        }

        private IEnumerator OpenChest(Action onAnimationComplete, string animationParam)
        {
            yield return new WaitForSeconds(_openChestTimeout);
            AnimatorTween.Create(gameObject)
                         .Trigger(animationParam, animationParam, () => {
                             gameObject.SetActive(false);
                             onAnimationComplete?.Invoke();
                         });
        }

        private string GetAnimationParam(LootBoxId lootBoxId)
        {
            return lootBoxId switch {
                    LootBoxId.LootBoxCommon => OPEN_COMMON_PARAM_NAME,
                    LootBoxId.LootBoxRare => OPEN_RARE_PARAM_NAME,
                    _ => throw new ArgumentOutOfRangeException(nameof(lootBoxId), lootBoxId, null)
            };
        }
    }
}