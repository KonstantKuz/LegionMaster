using System;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Core.Config;
using LegionMaster.HyperCasual.Config;
using LegionMaster.Location.Arena.Service;
using LegionMaster.UI.Screen.Squad.View;
using LegionMaster.Units.Component;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Service
{
    [PublicAPI]
    public class UnitFactory
    {
        private const string UNIT_PREFABS_PATH_ROOT = "Content/Location/Unit/";

        private Unit[] _unitPrefabs;

        [Inject]
        private readonly DiContainer _container;
        [Inject]
        private LocationObjectFactory _locationObjectFactory;
        [Inject]
        private HyperCasualUnitConfig _hyperCasualUnitConfig;

        public Unit LoadUnitForUi(string unitId, UnitType unitType, Transform unitRoot)
        {
            var unit = LoadUnitForMeta(unitId, unitType, unitRoot);
            unit.SetAiActive(false);
            return unit;
        }

        public Unit LoadDraggablePlayerUnit(string unitId,
                                            Transform unitRoot,
                                            Action<GameObject> mouseDown,
                                            Action<GameObject> mouseUp,
                                            UnitColliderParams colliderParams)
        {
            var unitObj = LoadUnitForUi(unitId, UnitType.PLAYER, unitRoot);
            unitObj.SetupCollider(colliderParams);
            var clickableUnit = unitObj.gameObject.AddComponent<DraggableObject>();
            clickableUnit.Init(mouseDown, mouseUp);
            return unitObj;
        }

        public Unit LoadUnitForMeta(string unitId, UnitType unitType, Transform unitRoot)
        {
            var prefab = GetUnitPrefab(unitId);
            var unit = _container.InstantiatePrefabForComponent<Unit>(prefab, unitRoot);
            SetUnitSize(unit.gameObject, unitId);
            unit.SetTeam(unitType);
            return unit;
        }

        public Unit LoadForBattle(IUnitModel unitModel, UnitType unitType, Transform spawnTransform)
        {
            var unitObj = _locationObjectFactory.CreateObject(unitModel.UnitId);
            SetUnitSize(unitObj, unitModel.UnitId);
            SetPositionAndRotation(unitObj, unitType, spawnTransform);
            var unit = unitObj.GetComponentInChildren<Unit>()
                       ?? throw new NullReferenceException($"Unit is null, objectId:= {unitModel.UnitId}, gameObject:= {unitObj.name}");
            Configure(unit, unitModel, unitType);

            return unit;
        }

        private static void Configure(Unit unit, IUnitModel unitModel, UnitType unitType)
        {
            unit.SetTeam(unitType);
            unit.Init(unitModel);
        }

        private void SetUnitSize(GameObject unitObj, string unitId)
        {
            if (!AppConfig.IsHyperCasual) {
                return;
            }
            var localScale = unitObj.transform.localScale;
            var scaleIncrement = _hyperCasualUnitConfig.GetScaleIncrement(unitId);
            unitObj.transform.localScale = new Vector3(localScale.x + scaleIncrement, localScale.y + scaleIncrement, localScale.z + scaleIncrement);
        }

        private void SetPositionAndRotation(GameObject unit, UnitType unitType, Transform spawnTransform)
        {
            unit.transform.SetPositionAndRotation(spawnTransform.transform.position, unitType.GetSpawnRotation(spawnTransform));
        }

        private Unit GetUnitPrefab(string unitId)
        {
            return UnitPrefabs.FirstOrDefault(p => p.ObjectId == unitId)
                   ?? throw new NullReferenceException($"Unit prefab is null, unitId:= {unitId}");
        }

        private Unit[] UnitPrefabs
        {
            get { return _unitPrefabs ??= Resources.LoadAll<Unit>(UNIT_PREFABS_PATH_ROOT); }
        }
    }
}