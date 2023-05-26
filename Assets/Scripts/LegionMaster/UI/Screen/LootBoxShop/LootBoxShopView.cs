using LegionMaster.Extension;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class LootBoxShopView : MonoBehaviour
    {
        public const string LOOTBOX_PREFABS_PATH = "Content/UI/LootBoxShop/";

        [SerializeField]
        private Transform _placementRoot;
        [Inject]
        private DiContainer _container;
        
        public void Init(LootBoxShopScreenModel model)
        {
            RemoveAllCreatedObjects();
            model.Placements.ForEach(it =>
            {
                var placementPrefab = Resources.Load<GameObject>(GetPlacementPrefabPath(it.PlacementId));
                var placementPanel = _container.InstantiatePrefabForComponent<LootBoxPlacementPanel>(placementPrefab, _placementRoot);
                
                placementPanel.Init(it);
            });
        }
        public void OnDisable()
        {
            RemoveAllCreatedObjects();
        }
        private void RemoveAllCreatedObjects()
        {
            _placementRoot.DestroyAllChildren();
        }

        private string GetPlacementPrefabPath(string placementId)
        {
            return LOOTBOX_PREFABS_PATH + placementId;
        }
    }
}