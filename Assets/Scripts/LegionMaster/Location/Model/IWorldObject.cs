
using UnityEngine;

namespace LegionMaster.Location.Model
{
    public interface IWorldObject 
    {
        ObjectType ObjectType { get; }
        string ObjectId { get; }
        GameObject GameObject { get; }
    }
}