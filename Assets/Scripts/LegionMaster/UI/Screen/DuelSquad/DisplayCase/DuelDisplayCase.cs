using LegionMaster.UI.Screen.DuelSquad.Model;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.DisplayCase
{
    public class DuelDisplayCase : MonoBehaviour
    {
        [SerializeField] private UnitDisplayCase _unitDisplayCase;
        [SerializeField] private DisplayCaseUpdateButton _displayCaseUpdateButton; 
        [SerializeField] private DisplayCaseItemPositions _displayCaseItemPositions;
        public void Init(IReactiveProperty<DisplayCaseUnitCollectionModel> units, DisplayCaseUpdateButtonModel buttonModel)
        {
            Show();
            _unitDisplayCase.Init(units);
            _displayCaseUpdateButton.Init(buttonModel);
        }
        public void Hide() => gameObject.SetActive(false);
        private void Show() => gameObject.SetActive(true);
        public UnitDisplayCase UnitDisplayCase => _unitDisplayCase; 
        public DisplayCaseItemPositions DisplayCaseItemPositions => _displayCaseItemPositions;

    }
}