using LegionMaster.Location.GridArena;
using LegionMaster.UI.Screen.Squad.SquadSetup;

namespace LegionMaster.UI.Screen.Squad.FreeCellProvider
{
    public class RandomFreeCellProvider : IFreeCellProvider
    {
        private readonly PlayerSquadSetup _playerSquadSetup;

        public RandomFreeCellProvider(PlayerSquadSetup playerSquadSetup)
        {
            _playerSquadSetup = playerSquadSetup;
        }

        public GridCell GetFreeCell()
        {
            return _playerSquadSetup.RandomFreeCell;
        }
    }
}