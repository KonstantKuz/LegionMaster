using UnityEngine;

namespace LegionMaster.UI.Screen.Campaign
{
    [RequireComponent(typeof(ScreenSwitcher))]
    public class CampaignScreen : BaseScreen
    {
        public const ScreenId CAMPAIGN_SCREEN = ScreenId.Campaign;
        public override ScreenId ScreenId => CAMPAIGN_SCREEN; 
        public override string Url => ScreenName;

    }
}