using UnityEngine;

namespace LegionMaster.Units.Component.Ai
{
    public abstract class AiBehaviorBase: ScriptableObject
    {
        public abstract void ProcessTimer(AiUnit aiUnit);
    }
}