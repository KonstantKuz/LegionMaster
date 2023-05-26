using JetBrains.Annotations;
using LegionMaster.UI.Screen.Footer;
using UnityEngine;

namespace LegionMaster.UI.Screen.Menu
{
    [RequireComponent(typeof(ScreenSwitcher))]
    public class MenuScreen : BaseScreen
    {
        public const ScreenId MENU_SCREEN = ScreenId.Menu;
        public override ScreenId ScreenId => MENU_SCREEN;
        public override string Url => ScreenName;

        [SerializeField] private FooterPresenter _footer;
        
        [PublicAPI]
        public void Init(bool showFooter)
        {
            _footer.gameObject.SetActive(showFooter);
        } 
        [PublicAPI]
        public void Init()
        {
            Init(true);
        }

        private void OnAfterScreenSwitched(string url)
        {
            _footer.SetCurrentScreen(url);
        }

        protected override void Awake()
        {
            base.Awake();
            GetComponent<ScreenSwitcher>().OnAfterScreenSwitched += OnAfterScreenSwitched;
        }

        private void OnDestroy()
        {
            GetComponent<ScreenSwitcher>().OnAfterScreenSwitched -= OnAfterScreenSwitched;
        }
    }
}