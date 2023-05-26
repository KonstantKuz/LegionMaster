using LegionMaster.Units.Component.Vfx;
using LegionMaster.Units.Effect.Config;

namespace LegionMaster.Units.Effect.Effects
{
    public class StunEffect : BaseEffect
    {
        private VfxPlayer _vfxPlayer;
        public override EffectType EffectType => EffectType.Stun;
        
        public override void Run()
        {
            base.Run();
            _vfxPlayer = _ownUnit.GetComponent<VfxPlayer>();
            _ownUnit.UnitStateMachine.SetDoNothingState();
            _vfxPlayer.PlayVfx(VfxType.Stun);
        }
        public override void Stop()
        {
            base.Stop();
            _ownUnit.UnitStateMachine.SetIdleState();
            _vfxPlayer.StopVfx(VfxType.Stun);
          
        }
    }
}