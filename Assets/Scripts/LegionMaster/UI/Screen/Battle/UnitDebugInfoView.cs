using System.Linq;
using LegionMaster.Extension;
using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Config;
using LegionMaster.Units.Model;
using TMPro;
using UnityEngine;

namespace LegionMaster.UI.Screen.Battle
{
    public class UnitDebugInfoView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        public void Init(IUnitModel model)
        {
            _text.text =
                BuildUnitDescriptionText(model);
            
            gameObject.SetActive(true);
        }

        private static string BuildUnitDescriptionText(IUnitModel model)
        {
            return @$"Id: {model.UnitId}
Health: {model.UnitHealth.StartingHealth}
Armor: {model.Armor}
Attack: {model.UnitAttack.Attack}
MoveSpeed: {model.MoveSpeed}
DodgeChance: {model.DodgeChance}
AttackRange: {model.UnitAttack.AttackRangeInCells}
AttackInterval: {model.UnitAttack.AttackInterval}
AttackType: {string.Join(",", model.UnitAttack.AttackTypes)}
CritChance: {model.UnitAttack.CriticalChance}
CritDamage: {model.UnitAttack.CriticalDamage}
HealthPerHit: {model.UnitHealth.RecoveryPerHit}
HealthPerAttack: {model.UnitHealth.RecoveryPerAttack}
HealthPerDeath: {model.UnitHealth.RecoveryPerDeath}
HealthPerSecond: {model.UnitHealth.RecoveryPerSecond}
" + string.Join("\n",
                       EnumExt.Values<AttackType>().Select(it =>
                           $"{AttackConfig.GetDamageBuffFieldName(it)}: {model.UnitAttack.GetDamageBuff(it)}")) +
                   "\n" +
                   string.Join("\n",
                       EnumExt.Values<AttackType>().Select(it =>
                           $"{UnitConfig.GetResistanceFieldName(it)}: {model.GetResistance(it)}")) + "\n";
        }

        private void OnEnable()
        {
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }
    }
}