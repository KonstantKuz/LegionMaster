using System;

namespace LegionMaster.UI.Screen.DuelSquad.Model
{
    public struct DisplayCaseUnitId : IEquatable<DisplayCaseUnitId>
    {
        private const int DISPLAY_CASE_UNIT_STARTING_STAR = 0;
        public string UnitId { get; }
        public int PlaceId { get; }       
        public int Star { get; }

        public DisplayCaseUnitId(string unitId, int placeId)
        {
            UnitId = unitId;
            PlaceId = placeId;
            Star = DISPLAY_CASE_UNIT_STARTING_STAR;
        }

        public bool Equals(DisplayCaseUnitId other)
        {
            return UnitId == other.UnitId && PlaceId == other.PlaceId && Star == other.Star;
        }

        public override bool Equals(object obj)
        {
            return obj is DisplayCaseUnitId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked {
                int hashCode = (UnitId != null ? UnitId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PlaceId;
                hashCode = (hashCode * 397) ^ Star;
                return hashCode;
            }
        }
    }
}