using LegionMaster.UI.Screen;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class SwitchScreenButton : MonoBehaviour
    {
        [Inject] private ScreenSwitcher _screenSwitcher;
        [SerializeField] private BaseScreen _screen;
        [SerializeField] private bool _immediately;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _screenSwitcher.SwitchTo(_screen.Url, !_immediately);
        }
    }
}