using System;
using LegionMaster.Extension;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.Units.Model;
using LegionMaster.Units.Model.Meta;
using LegionMaster.Util;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Description.View
{
    public class DescriptionView : MonoBehaviour
    {
        private const string RARITY_COLORS_CONFIG_PATH = "Content/UI/RarityColorsConfig";
        
        private const string LEVEL_LOCALIZATION_ID = "lvl";

        [SerializeField] private Image _unitIcon;
        [SerializeField] private StatisticsItemPanel _mainStatisticsItemPrefab;
        [SerializeField] private Transform _mainStatisticRoot;
        
        [SerializeField] private StatisticsItemPanel _characterNamePanel; 
        [SerializeField] private StatisticsItemPanel _rarityPanel;
        [SerializeField] private StatisticsItemPanel _characterLevelPanel;

        [Inject] private DiContainer _container;
        
        private CompositeDisposable _disposable;      
        
        public void Init(IObservable<UnitDescriptionModel> unitDescription)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            unitDescription.Subscribe(Configure).AddTo(_disposable);
        }
        
        private void Configure(UnitDescriptionModel unitDescriptionModel)
        {
            RemoveAllCreatedObjects();
            
            CreateMainStatisticsItems(unitDescriptionModel.Model);

            SetUnitIcon(unitDescriptionModel.UnitId);
            SetRarityColor(unitDescriptionModel.Model.RankedUnit.RarityType);
            
            _characterNamePanel.Init(unitDescriptionModel.UnitId); 
            _rarityPanel.Init(unitDescriptionModel.Model.RankedUnit.RarityType.ToString());
            _characterLevelPanel.InitFormatted(LEVEL_LOCALIZATION_ID, new object[] {unitDescriptionModel.Model.RankedUnit.Level});
        }

        private void SetUnitIcon(string unitId)
        {
            var iconPath = IconPath.GetUnitVertical(unitId);
            _unitIcon.sprite = Resources.Load<Sprite>(iconPath);
        }

        private void SetRarityColor(RarityType rarityType)
        {
            var colorsConfig = Resources.Load<RarityColorsConfig>(RARITY_COLORS_CONFIG_PATH);
            _rarityPanel.SetTextColor(colorsConfig.GetColorBy(rarityType));
        }

        private void CreateMainStatisticsItems(UnitModel model)
        {
            CreateMainStatisticsItem(StatisticsView.ATTACK_LABEL, model.UnitAttack.Attack.ToString());
            CreateMainStatisticsItem(StatisticsView.ARMOR_LABEL, model.Armor.ToString());
            CreateMainStatisticsItem(StatisticsView.HEALTH_LABEL, model.UnitHealth.StartingHealth.ToString());
        }
        private void CreateMainStatisticsItem(string localizationId, string value)
        {
            var item = _container.InstantiatePrefabForComponent<StatisticsItemPanel>(_mainStatisticsItemPrefab, _mainStatisticRoot);
            item.Init(localizationId, value);
        }
        public void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
            RemoveAllCreatedObjects();
        }
        private void RemoveAllCreatedObjects()
        {
            _mainStatisticRoot.DestroyAllChildren();
        }
    }
}