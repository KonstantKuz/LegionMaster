using System;
using LegionMaster.UI.HUD;
using LegionMaster.UI.Loader;
using LegionMaster.UI.Screen.Battle.UnitHud;
using LegionMaster.Units.Component.HealthEnergy;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.Units.Component.Hud
{
    public class HudOwner : MonoBehaviour, IInitializableComponent, IUnitHudOwner, IStarBarOwner
    {
        [SerializeField] private Transform _hudPosition;
        
        [Inject] private HudContainer _hudContainer;
        [Inject] private UILoader _uiLoader;
        
        private readonly IntReactiveProperty _stars = new IntReactiveProperty();
        private IEnergyBarOwner _energyBarOwner;
        private IHealthBarOwner _healthBarOwner;
        private Unit _unit;
        private UnitHudPresenter _unitHud;
        
        public IObservable<int> Stars => _stars;
        public bool StarBarEnabled => _unit.UnitModel.StarBarVisible;

        public Transform HudPosition => _hudPosition;
        public event Action OnRemoveHud;
        public IHealthBarOwner HealthBarOwner => _healthBarOwner ??= _unit.GetComponent<IHealthBarOwner>();
        public IStarBarOwner StarBarOwner => this;        
        
        private IEnergyBarOwner EnergyBarOwner => _energyBarOwner ??= _unit.GetComponent<IEnergyBarOwner>();
        
        private void OnDestroy()
        {
            CleanUp();
        }

        public void Init(Unit unit)
        {
            CleanUp();
            _unit = unit;
            HealthBarOwner.Init(unit);
            EnergyBarOwner.Init(unit);
            _stars.Value = unit.UnitModel.Star;

            if (unit.UnitModel.HudVisible)
            {
                _unit.OnDeath += OnUnitDeath;
                CreateHud();
            }
        }

        public UnitHudOwnerType OwnerType => _unit.UnitType switch
        {
            UnitType.AI => UnitHudOwnerType.Enemy,
            UnitType.PLAYER => UnitHudOwnerType.Player,
            _ => throw new ArgumentOutOfRangeException()
        };

        private void OnUnitDeath(Unit obj)
        {
            OnRemoveHud?.Invoke();
            CleanUp();
        }

        private void CleanUp()
        {
            if (_unitHud != null)
            {
                Destroy(_unitHud.gameObject);
                _unitHud = null;
            }

            if (_unit != null)
            {
                _unit.OnDeath -= OnUnitDeath;
                _unit = null;
            }
        }

        private void CreateHud()
        {
            _unitHud = _uiLoader.Load(UIModel<UnitHudPresenter, IUnitHudOwner>.Create(this)
                .Container(_hudContainer.transform)
                .Path(OwnerType.GetPrefabPath()));
            _unitHud.Init(this);

            _unitHud.HealthBar.Init(HealthBarOwner);
            _unitHud.StarBar.Init(StarBarOwner);
            _unitHud.EnergyBar.Init(EnergyBarOwner);
        }
    }
}