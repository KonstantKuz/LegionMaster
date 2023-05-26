using LegionMaster.Extension;
using SuperMaxim.Core.Extensions;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class LootBoxPlacementPanel : MonoBehaviour
    {
        [SerializeField]
        private Transform _lootBoxRoot;
        private CompositeDisposable _disposable;
        [Inject]
        private DiContainer _container;
        public void Init(LootBoxPlacementModel placementModel)
        {
            RemoveAllCreatedObjects();
            _disposable = new CompositeDisposable();
            placementModel.Products.ForEach(it =>
            {
                var panelPrefab = Resources.Load<GameObject>(GetPanelPrefabPath(placementModel.PlacementId));
                var lootBox = _container.InstantiatePrefabForComponent<ProductItemView>(panelPrefab, _lootBoxRoot);
                it.Subscribe(product => lootBox.Init(product)).AddTo(_disposable);
            });
        }

        public void OnDisable()
        {
            RemoveAllCreatedObjects();
        }

        private void RemoveAllCreatedObjects()
        {
            _lootBoxRoot.DestroyAllChildren();
            _disposable?.Dispose();
            _disposable = null;
        }

        private string GetPanelPrefabPath(string placementId)
        {
            return LootBoxShopView.LOOTBOX_PREFABS_PATH + "LootBox" + placementId;
        }
    }
}