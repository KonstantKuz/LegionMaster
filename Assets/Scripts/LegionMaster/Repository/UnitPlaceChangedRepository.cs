using LegionMaster.UI.Screen.Squad.UnitPlaceChanged;

namespace LegionMaster.Repository
{
    public class UnitPlaceChangedRepository : LocalPrefsSingleRepository<UnitPlaceChanged>
    {
        protected UnitPlaceChangedRepository() : base("unitPlaceChanged")
        {
        }
    }
}