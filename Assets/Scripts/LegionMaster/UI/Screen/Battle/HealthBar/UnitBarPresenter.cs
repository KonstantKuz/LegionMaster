using LegionMaster.UI.Components;
using LegionMaster.Units.Component.HealthEnergy;
using UnityEngine;

namespace LegionMaster.UI.Screen.Battle.HealthBar
{
    public class UnitBarPresenter : MonoBehaviour, IUiInitializable<IUnitBarOwner>
    {
        [SerializeField] 
        private UnitBarView _barView;
        
        private UnitBarModel _model;
        
        public void Init(IUnitBarOwner healthBarOwner)
        {
            _model = new UnitBarModel(healthBarOwner);
            _barView.Init(_model);
        }
        private void OnDisable()
        {
            _model = null;
        }
    }
}