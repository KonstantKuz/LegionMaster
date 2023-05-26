using JetBrains.Annotations;

namespace LegionMaster.Modifiers
{
    public interface IModifiableParameterOwner
    {
        [NotNull] IModifiableParameter GetParameter(string name);
        void AddParameter(IModifiableParameter parameter);
    }
}