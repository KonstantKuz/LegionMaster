using System.Collections.Generic;
using System.Linq;
using LegionMaster.Units;
using LegionMaster.Units.Component;
using LegionMaster.Units.Service;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Main
{
    public class UnitGroupView : MonoBehaviour
    {
        [SerializeField] private Transform _unitParent;
        [SerializeField] private AnimationCurve _scale;
        [SerializeField] private AnimationCurve _offset;
        [Inject] private UnitFactory _unitFactory;

        private readonly List<Unit> _units = new List<Unit>();

        public void Init(IList<string> unitIds)
        {
            CleanUp();
            gameObject.SetActive(true);
            PlaceUnits(unitIds);
            ResizeUnitGroup();
        }

        private void PlaceUnits(IList<string> unitIds)
        {
            var locatorCount = _unitParent.transform.childCount;
            if (locatorCount < unitIds.Count)
            {
                Debug.LogError($"There is more units {unitIds.Count} than locators for units in {gameObject.name} {locatorCount}. Some units won't be displayed");
            }
            for (int idx = 0; idx < unitIds.Count && idx < locatorCount; idx++)
            {
                var unitId = unitIds[idx];
                var unit = _unitFactory.LoadUnitForUi(unitId, UnitType.PLAYER, _unitParent.transform.GetChild(idx));
                _units.Add(unit);
            }
        }

        private void ResizeUnitGroup()
        {
            var groupSize = GetGroupSizeByX();
            _unitParent.transform.localScale = _scale.Evaluate(groupSize) * Vector3.one;
            _unitParent.transform.localPosition = _offset.Evaluate(groupSize) * Vector3.forward;
        }

        private float GetGroupSizeByX()
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            for (int idx = 0; idx < _unitParent.transform.childCount; idx++)
            {
                var tr = _unitParent.transform.GetChild(idx).transform;
                if (tr.childCount == 0) continue;
                var pos = tr.position;
                min = Mathf.Min(min, pos.x);
                max = Mathf.Max(max, pos.x);
            }

            return Mathf.Max(0, max - min);
        }

        private void CleanUp()
        {
            _units.Where(unit => unit).ForEach(unit => Destroy(unit.gameObject));
            _units.Clear();
        }

        public void Hide()
        {
            CleanUp();
            gameObject.SetActive(false);
        }
    }
}