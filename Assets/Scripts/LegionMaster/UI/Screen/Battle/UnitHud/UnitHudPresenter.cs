using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Battle.HealthBar;
using LegionMaster.UI.Screen.Battle.StarBar;
using LegionMaster.Units.Component.Hud;
using UnityEngine;

namespace LegionMaster.UI.Screen.Battle.UnitHud
{
    public class UnitHudPresenter : MonoBehaviour, IUiInitializable<IUnitHudOwner>
    {
        [SerializeField] private UnitHudView _view;
        [SerializeField] private UnitBarPresenter _healthBarPresenter;
        [SerializeField] private UnitBarPresenter _energyBarPresenter;

        private UnitHudModel _model;

        public UnitBarPresenter HealthBar => _healthBarPresenter;
        public UnitBarPresenter EnergyBar => _energyBarPresenter;
        public StarBarPresenter StarBar => GetComponent<StarBarPresenter>();

        private void OnDisable()
        {
            _view.Term();
            _model?.Dispose();
            _model = null;
        }

        public void Init(IUnitHudOwner owner)
        {
            _model?.Dispose();
            _model = new UnitHudModel(owner);
            _view.Init(_model);
        }
    }
}