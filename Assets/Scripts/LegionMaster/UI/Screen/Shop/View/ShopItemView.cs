using LegionMaster.UI.Screen.LootBoxShop;
using LegionMaster.UI.Screen.Shop.Model;
using ModestTree;
using TMPro;
using UnityEngine;

namespace LegionMaster.UI.Screen.Shop.View
{
    [RequireComponent(typeof(ProductItemView))]
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField] private GameObject _bonusContainer;
        [SerializeField] private TMP_Text _bonusText;
        
        private ProductItemView _itemView;
        private ProductItemView ItemView => _itemView ??= GetComponent<ProductItemView>();
        public void Init(ShopProductModel productModel)
        {
            ItemView.Init(productModel.Product);
            TrySetBonus(productModel.Bonus);
        }

        private void TrySetBonus(string bonus)
        {
            if (_bonusText == null)
            {
                return;
            }
            
            _bonusText.SetText(bonus);
            _bonusContainer.SetActive(!bonus.IsEmpty());
        }
    }
}