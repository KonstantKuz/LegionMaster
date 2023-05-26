using System;
using System.Collections.Generic;
using LegionMaster.Extension;
using LegionMaster.Reward.Model;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Debriefing;
using LegionMaster.UI.Screen.Description.View;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Dialog.PurchaseInfo
{
    public class PurchaseInfoView : MonoBehaviour
    {
        public const string INFOBOX_PREFAB_PATH = "Content/UI/Shop/PurchaseItemInfoBox";
        
        private readonly Dictionary<RewardItem, RectTransform> _rewardPositions = new Dictionary<RewardItem, RectTransform>();
        
        [SerializeField] private TextMeshProLocalization _labelText;
        [SerializeField] private Transform _purchaseItemsRoot;
        [SerializeField] private ButtonWithPrice _buyButton;
        [SerializeField] private ActionButton _closeButton;

        [Inject] private DiContainer _container;

        public void Init(PurchaseInfoModel purchaseInfoModel, Action onClose)
        {
            ClearView();
            
            _labelText.LocalizationId = purchaseInfoModel.LabelId;
            purchaseInfoModel.Rewards.ForEach(SpawnItemInfoView);
            
            _buyButton.Init(purchaseInfoModel.PriceButton, purchaseInfoModel.OnBuyClick);
            _closeButton.Init(onClose);
        }

        private void ClearView()
        {
            _purchaseItemsRoot.DestroyAllChildren();
        }
        private void SpawnItemInfoView(RewardItemModel reward)
        {
            var viewPrefab = Resources.Load<GameObject>(INFOBOX_PREFAB_PATH);
            var itemView = _container.InstantiatePrefabForComponent<RewardItemPanel>(viewPrefab, _purchaseItemsRoot);
            itemView.Init(reward);
            _rewardPositions[reward.RewardItem] = itemView.GetComponent<RectTransform>();
        }
        
        public RectTransform RewardPosition(RewardItem rewardItem) => _rewardPositions[rewardItem];
    }
}