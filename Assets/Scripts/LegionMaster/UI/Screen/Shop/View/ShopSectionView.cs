using System;
using System.Linq;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Shop.Model;
using LegionMaster.Util;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Shop.View
{
    public class ShopSectionView : MonoBehaviour
    {
        public const string PREFAB_PATH_PREFIX = "Content/UI/Shop/";
        
        [SerializeField] private Image _sectionLabelBack;
        [SerializeField] private TextMeshProLocalization _sectionLabel;
        [SerializeField] private Transform _boxesRoot;
        [SerializeField] private Transform _bannersRoot;
        
        [Inject] private DiContainer _container;

        public void Init(ShopSectionModel sectionModel)
        {
            TryUpdateSectionLabel(sectionModel);
            TryActivateViewTypeRoots(sectionModel);
            
            sectionModel.Products.ForEach(SpawnItemView);
        }

        private void TryUpdateSectionLabel(ShopSectionModel sectionModel)
        {
            var sectionLabelIcon = Resources.Load<Sprite>(IconPath.GetShopSectionLabel(sectionModel.SectionId));
            if (sectionLabelIcon == null)
            {
                _sectionLabelBack.gameObject.SetActive(false);
                Debug.Log("There is no icon for shop section. Disabling shop section label.");
                return;
            }
            
            _sectionLabel.LocalizationId = $"{sectionModel.SectionId}";
            _sectionLabelBack.sprite = sectionLabelIcon;
        }

        private void TryActivateViewTypeRoots(ShopSectionModel sectionModel)
        {
            bool hasBoxViews = sectionModel.Products.Any(product => product.ViewType == ShopItemViewType.Box);
            _boxesRoot.gameObject.SetActive(hasBoxViews);

            bool hasBannerViews = sectionModel.Products.Any(product => product.ViewType == ShopItemViewType.Banner);
            _bannersRoot.gameObject.SetActive(hasBannerViews);
        }

        private void SpawnItemView(ShopProductModel productModel)
        {
            string prefabPath = PREFAB_PATH_PREFIX + productModel.ViewPrefabId;
            var viewPrefab = Resources.Load<GameObject>(prefabPath);
            var itemView = _container.InstantiatePrefabForComponent<ShopItemView>(viewPrefab, _bannersRoot);
            itemView.Init(productModel);
            itemView.transform.SetParent(GetParent(productModel.ViewType));
        }

        private Transform GetParent(ShopItemViewType itemViewType)
        {
            switch (itemViewType)
            {
                case ShopItemViewType.Box:
                    return _boxesRoot;
                case ShopItemViewType.Banner:
                    return _bannersRoot;
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemViewType), itemViewType, null);
            }
        }
    }
}