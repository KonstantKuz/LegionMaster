using UnityEngine;

namespace LegionMaster.Units.Component.Ai
{
    public interface IMoving  
    {
        Vector3 MovementDirection { get; }
        Vector3 LookAtDirection { get; }
        bool IsMoving { get; }
    }
}