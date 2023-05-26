using System.Linq;
using EasyButtons;
using LegionMaster.Units;
using LegionMaster.Units.Component;
using LegionMaster.Units.Component.Ai;
using LegionMaster.Units.Config;
using LegionMaster.Units.Effect;
using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Model;
using LegionMaster.Units.Model.Battle;
using LegionMaster.Units.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Test.UnitTestScene
{
    public class UnitTestSceneSetup : MonoBehaviour
    {
        [SerializeField]
        private string _unitId;
        [SerializeField]
        private UnitType _unitType = UnitType.PLAYER;
        [SerializeField]
        private bool _doNothing;

        [Inject]
        private UnitModelBuilder _unitModelBuilder;
        [Inject]
        private UnitFactory _unitFactory;
        [Inject]
        private UnitCollectionConfig _unitCollectionConfig;
        
        private string _currentUnitId;
        private Unit _activeUnit;

        private void OnEnable()
        {
            LoadUnit(_unitId);
        }

        private void LoadUnit(string unitId)
        {
            _currentUnitId = unitId;
            var model = _unitModelBuilder.BuildInitialUnit(unitId);
            var battleModel = new UnitBattleModel(model) {
                    AiEnabled = true,
                    HudVisible = true,
            };
            _activeUnit = _unitFactory.LoadUnitForMeta(unitId, _unitType, transform);
            _activeUnit.Init(battleModel);
            _activeUnit.IsAlive = true;
            if (_doNothing) {
                _activeUnit.GetComponent<UnitStateMachine>().SetDoNothingState(); 
            }
        }

        [Button]
        private void NextUnit()
        {
            var nextUnitId = GetNextUnitId(_currentUnitId);
            RemoveUnit();
            LoadUnit(nextUnitId);
        }

        [Button]
        private void PrevUnit()
        {
            var nextUnitId = GetPrevUnitId(_currentUnitId);
            RemoveUnit();
            LoadUnit(nextUnitId);
        }

        [Button]
        private void AddEffect(EffectType effectType)
        {
            var effectOwner = _activeUnit.GetComponent<IEffectOwner>();
            effectOwner.AddEffect(effectType);
        }

        [Button]
        private void RemoveEffect(EffectType effectType)
        {
            var effectOwner = _activeUnit.GetComponent<IEffectOwner>();
            effectOwner.RemoveEffect(effectType);
        }

        private string GetPrevUnitId(string unitId)
        {
            var ids = _unitCollectionConfig.AllUnitIds.ToList();
            var prevIdx = ids.IndexOf(unitId) - 1;
            if (prevIdx < 0) prevIdx = ids.Count - 1;
            return ids[prevIdx];
        }

        private string GetNextUnitId(string unitId)
        {
            var ids = _unitCollectionConfig.AllUnitIds.ToList();
            var nextIdx = ids.IndexOf(unitId) + 1;
            if (nextIdx >= ids.Count) nextIdx = 0;
            return ids[nextIdx];
        }

        private void RemoveUnit()
        {
            if (_activeUnit == null) {
                return;
            }
            _activeUnit.Kill();
        }
    }
}