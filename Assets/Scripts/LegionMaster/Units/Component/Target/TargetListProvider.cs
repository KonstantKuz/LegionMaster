using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LegionMaster.Units.Component.Target
{
    [PublicAPI]
    public class TargetListProvider
    {
        private readonly HashSet<ITarget> _targets = new HashSet<ITarget>();

        public void Add(ITarget target)
        {
            _targets.Add(target);
        }

        public void Remove(ITarget target)
        {
            _targets.Remove(target);
        }

        public IEnumerable<ITarget> AllTargets => _targets;

        public ITarget FindById(string targetId) => _targets.FirstOrDefault(it => it.TargetId == targetId);
    }
}