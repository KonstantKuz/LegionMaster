using UnityEngine;

namespace LegionMaster.Units.Config
{
    [CreateAssetMenu(fileName = "UnitColliderParams", menuName = "LegionMaster/UnitColliderParams")]
    public class UnitColliderParams : ScriptableObject
    {
        public float CenterHeight;
        public float SizeMultiplier;
    }
}
