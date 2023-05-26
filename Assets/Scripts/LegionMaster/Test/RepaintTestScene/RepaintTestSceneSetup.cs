using LegionMaster.Extension;
using LegionMaster.Units.Component;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model;
using LegionMaster.Units.Service;
using UnityEngine;
using Zenject;

namespace LegionMaster.Test.RepaintTestScene
{
    public class RepaintTestSceneSetup : MonoBehaviour
    {
        [SerializeField] private float _offset = 30.0f;
        
        [Inject] private UnitCollectionConfig _unitCollectionConfig;
        [Inject] private UnitModelBuilder _unitModelBuilder;
        [Inject] private UnitFactory _unitFactory;        
        
        private void Awake()
        {
            var offsetX = 0.0f;
            foreach (var unitType in EnumExt.Values<UnitType>())
            {
                var offsetZ = 0.0f;
                foreach (var unitId in _unitCollectionConfig.AllUnitIds)
                {
                    var model = _unitModelBuilder.BuildInitialUnit(unitId);
                    var unit = _unitFactory.LoadUnitForMeta(unitId, unitType, transform);
                    unit.Init(model);
                    unit.transform.localPosition = new Vector3(offsetX, 0, offsetZ);
                    offsetZ += _offset;
                }

                offsetX += _offset;
            }
        }
    }
}