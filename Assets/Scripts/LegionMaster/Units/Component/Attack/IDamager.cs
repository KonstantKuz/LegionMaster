using UnityEngine;

namespace LegionMaster.Units.Component.Attack
{
    public interface IDamager
    {
        void DoDamage(GameObject target, bool isCritical);
    }
}