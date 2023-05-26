using LegionMaster.Location.GridArena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.UI.Screen.Squad.SquadSetup;

namespace LegionMaster.UI.Screen.Squad.FreeCellProvider
{
    public class FixedSequenceCellProvider : IFreeCellProvider
    {
        private readonly PlayerSquadSetup _playerSquadSetup;
        private readonly CellId[] _cellIds;
        private readonly IFreeCellProvider _randomCellProvider;
        
        private int _sequenceIdx = 0;
        public FixedSequenceCellProvider(PlayerSquadSetup playerSquadSetup, CellId[] cellIds)
        {
            _playerSquadSetup = playerSquadSetup;
            _cellIds = cellIds;
            _randomCellProvider = new RandomFreeCellProvider(_playerSquadSetup);
        }

        public GridCell GetFreeCell()
        {
            return _sequenceIdx >= _cellIds.Length ? 
                _randomCellProvider.GetFreeCell() : 
                _playerSquadSetup.GetGridCellById(_cellIds[_sequenceIdx++]);
        }
    }
}