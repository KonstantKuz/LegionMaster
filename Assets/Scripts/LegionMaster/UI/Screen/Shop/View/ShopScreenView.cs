using LegionMaster.Extension;
using LegionMaster.UI.Screen.Shop.Model;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Shop.View
{
    public class ShopScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject _shopSectionPrefab;
        [SerializeField] private Transform _shopSectionsRoot;
        [SerializeField] private ScrollRect _scrollRect;
        
        [Inject] private DiContainer _container;
        
        public void Init(ShopScreenModel model)
        {
            ClearView();
            RectTransform activeSection = null;
            model.SectionModels.ForEach(sectionModel =>
            {
                var shopSectionView = _container.InstantiatePrefabForComponent<ShopSectionView>(_shopSectionPrefab, _shopSectionsRoot);
                shopSectionView.Init(sectionModel);
                if (sectionModel.SectionId == model.ActiveSectionId.ToString())
                {
                    activeSection = shopSectionView.GetComponent<RectTransform>();
                }
            });
         
            RebuildLayout();
            ScrollToSection(activeSection);
        }

        private void ClearView()
        {
            _shopSectionsRoot.DestroyAllChildren();;
        }

        private void ScrollToSection(RectTransform section)
        {
            _scrollRect.ScrollToChild(section);
        }

        private void OnDisable()
        {
            ClearView();
        }

        private void RebuildLayout()
        {
            foreach (var layoutGroup in GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.transform as RectTransform);
            }
            _scrollRect.normalizedPosition = Vector2.one;
        }
    }
}