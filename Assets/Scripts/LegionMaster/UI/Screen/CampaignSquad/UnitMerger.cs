using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LegionMaster.UI.Screen.DuelSquad.SquadSetup;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.UI.Screen.Squad.Model;
using LegionMaster.Units;
using LegionMaster.Units.Component.Vfx;
using LegionMaster.Units.Model.Meta;
using UnityEngine;

namespace LegionMaster.UI.Screen.CampaignSquad
{
    public class UnitMerger : MonoBehaviour
    {
        [SerializeField] 
        private float _unitMergeTime = 0.2f; 
        [SerializeField] 
        private float _unitMergeVfxTime;
        
        private MergeablePlayerSquadSetup _squadSetup;
        private UnitCursor _unitCursor;
        
        public void Init(MergeablePlayerSquadSetup squadSetup, UnitCursor unitCursor)
        {
            _squadSetup = squadSetup;
            _unitCursor = unitCursor;
        }
        public bool CanMerge(string unitId, int star) => _squadSetup.FindPlacedUnit(unitId, star) != null;
        public void Merge(string unitId, int star)
        {
            var placedUnitModel = _squadSetup.GetPlacedUnit(unitId, star);
            var unitObj = _unitCursor.AttachedUnit;
            _unitCursor.Detach();
            var mergeObjects = new List<GameObject>();
            mergeObjects.Add(unitObj);
            
            var placedUnit = _squadSetup.GetUnitObject(placedUnitModel).GetComponent<Unit>();
            IncreaseStar(placedUnitModel, placedUnit);
            TryMergePlacedUnits(placedUnit, placedUnitModel, mergeObjects);
        }

        private void TryMergePlacedUnits(Unit fromUnit, PlacedUnitModel fromUnitModel, List<GameObject> mergeObjects)
        {
            var toUnitModel = _squadSetup.FindMergeableUnit(fromUnitModel);
            if (toUnitModel == null) {
                mergeObjects.Add(fromUnit.gameObject);
                StartCoroutine(PlayMerge(mergeObjects));
                return;
            }
            var toUnit = _squadSetup.FindUnitObjects(fromUnitModel.Id, fromUnitModel.Star).First(it => it != fromUnit);
            IncreaseStar(toUnitModel, toUnit);
            _squadSetup.SetCellFullState(fromUnitModel, false);
            _squadSetup.RemoveFromModelList(fromUnitModel);
            _squadSetup.Save();
            mergeObjects.Add(fromUnit.gameObject);
            TryMergePlacedUnits(toUnit, toUnitModel, mergeObjects);
        }
        private IEnumerator PlayMerge(List<GameObject> mergeObjects)
        {
            for (int i = 0; i < mergeObjects.Count - 1; i++) {
                var fromUnit = mergeObjects[i];  
                var toUnit = mergeObjects[i + 1];
                yield return fromUnit.transform.DOMove(toUnit.transform.position, _unitMergeTime).WaitForCompletion();
                toUnit.GetComponent<VfxPlayer>().PlayVfx(VfxType.MergeStars);
                Destroy(fromUnit);
                yield return new WaitForSeconds(_unitMergeVfxTime);
            }
            mergeObjects.Last().GetComponent<Unit>().UpdateHud();
            yield break;
        }
        private void IncreaseStar(PlacedUnitModel placedUnitModel, Unit unit)
        {
            placedUnitModel.Star++;
            var unitModel = (UnitModel) unit.UnitModel;
            unitModel.Star = placedUnitModel.Star;
            _squadSetup.Save();
        }
    }
}