using System;
using UnityEngine;
using static System.Int32;

namespace LegionMaster.Location.GridArena.Model
{
    [Serializable]
    public struct CellId
    {
        [SerializeField] private int _y;
        [SerializeField] private int _x;
        public int Y => _y;
        public int X => _x;
        public static CellId InvalidCellId => new CellId(MinValue, MinValue);
        public CellId(int y, int x)
        {
            _y = y;
            _x = x;
        }
        public bool Equals(CellId other)
        {
            return Y == other.Y && X == other.X;
        }
        public override bool Equals(object obj)
        {
            return obj is CellId other && Equals(other);
        }
        public static bool operator ==(CellId id1, CellId id2) => id1.Equals(id2);

        public static bool operator !=(CellId id1, CellId id2) => !(id1 == id2);
        public override string ToString()
        {
            return $"CellId:= y:={Y}, x:={X}";
        }
        public override int GetHashCode()
        {
            unchecked {
                return (Y * 397) ^ X;
            }
        }
    }
}