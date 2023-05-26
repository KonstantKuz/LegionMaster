using LegionMaster.UI.Screen.Shop;
using LegionMaster.UI.Screen.Shop.Model;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Header
{
    [RequireComponent(typeof(Button))]
    public class GoToShopButton : MonoBehaviour
    {
        [SerializeField] private ShopSectionId _shopSection;

        [Inject] private ScreenSwitcher _screenSwitcher;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _screenSwitcher.SwitchTo(ShopScreenPresenter.URL, false, _shopSection);
        }
    }
}