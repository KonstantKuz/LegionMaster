using LegionMaster.Cheats;
using LegionMaster.UI.Screen.Battle;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace LegionMaster.Units.Component
{
    [RequireComponent(typeof(Unit))]
    public class ClickableUnit : MonoBehaviour, IPointerClickHandler
    {
        private Unit _unit;
        [Inject(Optional = true)]
        private CheatsManager _cheatsManager;
        [Inject(Optional = true)]
        private UnitDebugInfoView _unitDebugInfoView;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_cheatsManager == null || !_cheatsManager.IsUnitInfoEnabled || _unit.UnitModel == null) return;
            _unitDebugInfoView.Init(_unit.UnitModel);
        }
    }
}