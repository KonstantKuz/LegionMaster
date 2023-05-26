namespace LegionMaster.UI.Screen.Description.Model
{
    public class ProgressButtonModel
    {
        public bool Enabled;
        public bool Interactable;
        public float CurrentValue;
        public float MaxValue;
        public float Progress => CurrentValue / MaxValue;
    }
}
