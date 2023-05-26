using System.Linq;
using LegionMaster.Extension;
using LegionMaster.UI.Screen.DuelSquad.Model;
using LegionMaster.Units.Component.Vfx;
using LegionMaster.Units.Config;
using LegionMaster.Units.Service;
using SuperMaxim.Core.Extensions;
using UniRx;
using UnityEngine;
using Zenject;
using Unit = LegionMaster.Units.Unit;

namespace LegionMaster.UI.Screen.DuelSquad.DisplayCase
{
    public class UnitDisplayCase : MonoBehaviour
    {
        [SerializeField] private Transform _unitsRoot;
        [SerializeField] private Transform _positionsRoot;  
        [SerializeField] private Vector3 _unitScale;
        [SerializeField] private UnitColliderParams _colliderParams;

        [Inject] private UnitFactory _unitFactory;
        
        private Transform[] _unitPositions;
        private CompositeDisposable _disposable;
        public void Init(IReactiveProperty<DisplayCaseUnitCollectionModel> model)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            model.Subscribe(LoadUnits).AddTo(_disposable);
        }
        public void PlaceUnit(GameObject unitObj, int positionNumber)
        {
            unitObj.transform.SetParent(_unitsRoot);
            var unitPosition = UnitPositions[positionNumber];
            unitObj.transform.localScale = _unitScale;
            unitObj.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
        }

        public void PlayRespawnEffect(DisplayCaseUnitId id)
        {
            var unit = _unitPositions[id.PlaceId];
            var vfxPlayer = unit.GetComponentInChildren<VfxPlayer>();
            vfxPlayer.PlayVfx(VfxType.DisplayCaseRespawn);
        }
        
        private void LoadUnits(DisplayCaseUnitCollectionModel displayCaseUnitCollection)
        {
            RemoveAllUnits();
            displayCaseUnitCollection.Units.Where(it => it.State.Value == DisplayedUnitState.NotTaken).ForEach(LoadAndPlaceUnit);
        }
        private void LoadAndPlaceUnit(DisplayCaseUnitModel model)
        {
            var unit = LoadUnit(model);
            PlaceUnit(unit.gameObject, model.Id.PlaceId);
        }

        private Unit LoadUnit(DisplayCaseUnitModel unit)
        {
            return _unitFactory.LoadDraggablePlayerUnit(unit.Id.UnitId, _unitsRoot, 
                                                       item => unit.OnStartDrag?.Invoke(item), 
                                                       item => unit.OnStopDrag?.Invoke(item), 
                                                       _colliderParams);
        }

        private void OnDisable()
        {
            RemoveAllUnits();
            _disposable?.Dispose();
            _disposable = null;
        }
        private void RemoveAllUnits()
        {
            _unitsRoot.transform.DestroyAllChildren();
        }
        private Transform[] UnitPositions => _unitPositions ??= _positionsRoot.gameObject.GetComponentsOnlyInChildren<Transform>();
    }
}