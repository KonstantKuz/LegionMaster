using LegionMaster.Units.Component.Attack;
using LegionMaster.Units.Component.Vfx;
using LegionMaster.Units.Component.Weapon;
using LegionMaster.Units.Effect.Config;

namespace LegionMaster.Units.Effect.Effects
{
    public class PyromaniacEffect : BaseEffect
    {
        private VfxPlayer _vfxPlayer;   
        private AttackUnit _attackUnit;   
        private PyromaniacDamager _pyromaniacDamager;    
        private BaseWeapon _pyromaniacWeapon;

        private IDamager _regularDamager;
        private BaseWeapon _regularWeapon;
        public override EffectType EffectType => EffectType.Pyromaniac;

        public override void Run()
        {
            base.Run();
            _vfxPlayer = _ownUnit.GetComponent<VfxPlayer>();     
            _attackUnit = _ownUnit.GetComponent<AttackUnit>();

            _regularDamager = _attackUnit.Damager;    
            _regularWeapon = _attackUnit.Weapon;
                    
            _pyromaniacDamager = _ownUnit.GetComponentInChildren<PyromaniacDamager>();
            _pyromaniacWeapon = _pyromaniacDamager.gameObject.GetComponent<BeamWeapon>();

            _attackUnit.Weapon = _pyromaniacWeapon;
            _attackUnit.Damager = _pyromaniacDamager;
            
            
            _vfxPlayer.PlayVfx(VfxType.PyromaniacAbility);
        }
        public override void Stop()
        {
            base.Stop();
            _vfxPlayer.StopVfx(VfxType.PyromaniacAbility);
            _attackUnit.Weapon = _regularWeapon;      
            _attackUnit.Damager = _regularDamager;
        }
    }
}