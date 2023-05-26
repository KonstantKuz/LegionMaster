using LegionMaster.UI.Screen.DuelSquad.DisplayCase;
using LegionMaster.UI.Screen.DuelSquad.Model;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.CampaignSquad.DisplayCase
{
    public class CampaignDisplayCase : MonoBehaviour
    {
        [SerializeField] private UnitDisplayCase _unitDisplayCase;
        
        [SerializeField] private DisplayCaseItemPositions _displayCaseItemPositions;
        
        public void Init(IReactiveProperty<DisplayCaseUnitCollectionModel> units)
        {
            Show();
            _unitDisplayCase.Init(units);
        }
        public void Hide() => gameObject.SetActive(false);
        private void Show() => gameObject.SetActive(true);
        
        public UnitDisplayCase UnitDisplayCase => _unitDisplayCase; 
        
        public DisplayCaseItemPositions DisplayCaseItemPositions => _displayCaseItemPositions;

    }
}