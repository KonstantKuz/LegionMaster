using System;
using LegionMaster.Extension;
using LegionMaster.UI.Screen.Description.Model;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Model.Meta;
using SuperMaxim.Core.Extensions;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.Description.View
{
    public class StatisticsView : MonoBehaviour
    {
        public const string ATTACK_LABEL = "attack"; 
        public const string ARMOR_LABEL = "armor";  
        public const string HEALTH_LABEL = "health";
        private const string MOVE_SPEED_LABEL = "move speed";
        private const string DODGE_CHANCE_LABEL = "dodge chance";
        private const string ATTACK_DISTANCE_LABEL = "attack distance";
        private const string ATTACK_INTERVAL_LABEL = "attack interval";
        private const string ATTACK_TYPE_LABEL = "attack type";
        private const string CRIT_CHANCE_LABEL = "crit chance";
        private const string CRIT_DAMAGE_LABEL = "crit damage";
        private const string DAMAGE_BUF_LABEL = " damage buf";
        private const string RESISTANCE_LABEL = " resistance"; 
        
        [SerializeField]
        private Transform _statisticRoot;
        [SerializeField]
        private StatisticsItemPanel _statisticsItemPrefab;

        private CompositeDisposable _disposable;

        public void Init(ReactiveProperty<UnitDescriptionModel> unitDescription)
        {
            _disposable = new CompositeDisposable();
            unitDescription.Subscribe(ConfigureStatisticsItem).AddTo(_disposable);
        }

        private void ConfigureStatisticsItem(UnitDescriptionModel unit)
        {
            RemoveAllStatisticsItems();
            CreateCommonStatisticsItems(unit.Model);
            CreateDamageBufStatisticsItems(unit.Model);
            CreateResistanceStatisticsItems(unit.Model);
        }
        private void CreateDamageBufStatisticsItems(UnitModel model)
        {
            foreach (AttackType attackType in Enum.GetValues(typeof(AttackType))) {
                CreateItem(attackType.ToString().ToLower() + DAMAGE_BUF_LABEL, model.UnitAttack.GetDamageBuff(attackType).ToString());
            }
        }     
        private void CreateResistanceStatisticsItems(UnitModel model)
        {
            foreach (AttackType attackType in Enum.GetValues(typeof(AttackType))) {
                CreateItem(attackType.ToString().ToLower() + RESISTANCE_LABEL, (100 * model.GetResistance(attackType)).ToString());
            }
        }   
        private void CreateCommonStatisticsItems(UnitModel model)
        {
            CreateItem(ATTACK_LABEL, model.UnitAttack.Attack.ToString());
            CreateItem(ARMOR_LABEL, model.Armor.ToString());
            CreateItem(HEALTH_LABEL, model.UnitHealth.StartingHealth.ToString());  
            CreateItem(MOVE_SPEED_LABEL, model.MoveSpeed.ToString());
            CreateItem(DODGE_CHANCE_LABEL, CreatePercent(model.DodgeChance));  
            CreateItem(ATTACK_DISTANCE_LABEL, model.UnitAttack.AttackRangeInCells.ToString()); 
            CreateItem(ATTACK_INTERVAL_LABEL, model.UnitAttack.AttackInterval.ToString());
            model.UnitAttack.AttackTypes
                 .ForEach(type => {
                     CreateItem(ATTACK_TYPE_LABEL, type.ToString().ToLower());
                 });
            CreateItem(CRIT_CHANCE_LABEL, CreatePercent(model.UnitAttack.CriticalChance));  
            CreateItem(CRIT_DAMAGE_LABEL, "x" + Math.Round(model.UnitAttack.CriticalDamage, 1));
        }

        private string CreatePercent(float value) => Math.Round(value, 0) + "%";
        private void CreateItem(string label, string value)
        {
            var item = Instantiate(_statisticsItemPrefab, _statisticRoot, false);
            item.Init(label, value);
        }
        
        public void OnDisable()
        {
            RemoveAllStatisticsItems();
            _disposable?.Dispose();
            
        }
        private void RemoveAllStatisticsItems()
        {
            _statisticRoot.DestroyAllChildren();
        }
    }
}