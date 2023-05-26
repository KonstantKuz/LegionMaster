using LegionMaster.Campaign.Session.Service;
using LegionMaster.Location.Session.Service;
using LegionMaster.UI.Screen.Campaign.Progress.Model;
using LegionMaster.UI.Screen.Campaign.Progress.View;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Campaign.Progress
{
    public class MatchProgressPresenter : MonoBehaviour
    {
        [SerializeField]
        private MatchProgressView _view;

        [Inject]
        private BattleSessionServiceWrapper _sessionService;
        
        private void OnEnable()
        {
            var model = MatchProgressModel.CreateForCampaign(_sessionService.GetImpl<CampaignSessionService>().CampaignBattleSession);
            _view.Init(model);
        }
    }
}