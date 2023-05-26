namespace LegionMaster.Modifiers
{
    public interface IModifiableParameter
    {
        string Name { get; }
        void Reset();
    }
}