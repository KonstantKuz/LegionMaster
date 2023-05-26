using LegionMaster.Units.Component.Vfx;
using LegionMaster.Units.Effect.Config;
using LegionMaster.Units.Service;
using ModestTree;
using UnityEngine;

namespace LegionMaster.Units.Effect.Effects
{
    public class BurningEffect : BaseEffect
    {
        private VfxPlayer _vfxPlayer;
        private bool _damageTimerStarted;
        private float _time;      
   
        public override EffectType EffectType => EffectType.Burning;

        public override void Run()
        {
            base.Run();
            Assert.IsNotNull(_caster, $"Caster unit is null, effect:= {EffectType}");
            _vfxPlayer = _ownUnit.GetComponent<VfxPlayer>();
            _vfxPlayer.PlayVfx(VfxType.Burning);
            StartDamageTimer();
        }
        public override void OnTick()
        {
            base.OnTick();
            if (_damageTimerStarted) {
                UpdateDamageTimer();
            }
        }
        public override void Stop()
        {
            base.Stop();
            _vfxPlayer.StopVfx(VfxType.Burning);
            _damageTimerStarted = false;
        }
        private void StartDamageTimer()
        {
            _time = 0.0f;
            _damageTimerStarted = true;
        }
        private void UpdateDamageTimer()
        {
            _time += Time.deltaTime;
            if (_time < _config.Params.DamagePeriod) {
                return;
            }
            DoDamage();
            _time = 0.0f;
        }
        private void DoDamage()
        {
            DamageService.DoDamage(_caster.UnitModel.UnitAttack, _ownUnit, false);
        }
    }
}