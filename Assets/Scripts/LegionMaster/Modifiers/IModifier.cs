namespace LegionMaster.Modifiers
{
    public interface IModifier
    {
        public void Apply(IModifiableParameterOwner parameterOwner);
    }
}