using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Shop.Model;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Shop.Widget
{
    public class ShopWidgetPresenter : MonoBehaviour
    {
        [SerializeField]
        private ActionButton _toShopButton;
        [SerializeField]
        private ShopSectionId _shopSectionId;
        
        [Inject] private ScreenSwitcher _screenSwitcher;
        
        private void Awake()
        {
            _toShopButton.Init(OnShowShop);
        }
        private void OnShowShop() => _screenSwitcher.SwitchTo(ShopScreenPresenter.URL, false, _shopSectionId);

    }
}