using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Squad;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Footer
{
    public class NoUnitPopupPresenter : MonoBehaviour
    {
        [SerializeField] private ActionButton _closeButton;
        [Inject] private ScreenSwitcher _screenSwitcher;
        [Inject] private Analytics.Analytics _analytics;        

        private void OnEnable()
        {
            _closeButton.Init(OnCloseButton);
            _analytics.ReportStartNoUnits();            
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void OnCloseButton()
        {
            Hide();
            _screenSwitcher.SwitchTo(SquadPresenter.URL, true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}