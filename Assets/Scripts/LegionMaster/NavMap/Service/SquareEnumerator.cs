using System.Collections;
using System.Collections.Generic;
using LegionMaster.Location.GridArena.Model;

namespace LegionMaster.NavMap.Service
{
    public class SquareEnumerator : IEnumerable
    {
        private CellId _center;
        private int _radius;

        public SquareEnumerator(CellId center, int radius)
        {
            _center = center;
            _radius = radius;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<CellId> GetEnumerator()
        {
            if (_radius == 0) {
                yield return _center;
                yield break;
            }
            for (var dx = _radius; dx >= -_radius; dx--) {
                yield return new CellId(_center.Y + _radius, _center.X + dx);
                yield return new CellId(_center.Y - _radius, _center.X + dx);
            }
            for (var dy = _radius - 1; dy >= -_radius + 1; dy--) {
                yield return new CellId(_center.Y + dy, _center.X + _radius);
                yield return new CellId(_center.Y + dy, _center.X - _radius);
            }
        }
    }
}