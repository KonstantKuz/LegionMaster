using LegionMaster.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.LootBoxShop
{
    public class ProductItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProLocalization _nameLabel;
        [SerializeField] private TMP_Text _countLabel;
        [SerializeField] private Image _productIcon;
        
        [SerializeField] private ShopButtonWithPrice _priceButton;

        public void Init(ProductItemModel product)
        {
            _priceButton.Init(product.PriceButton, product.OnBuyClick);
            NameLabel = product.ProductId;
            SetCountLabel(product.CountText);
            SetIcon(product.IconPath);
        }
        private void SetCountLabel(string value)
        {
            _countLabel.text = value;
        }
        private void SetIcon(string iconPath)
        {
            _productIcon.sprite = Resources.Load<Sprite>(iconPath);
        }
        private string NameLabel
        {
            set => _nameLabel.LocalizationId = value;
        }
    }
}