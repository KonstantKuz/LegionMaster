using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Core.Mode;
using LegionMaster.Extension;
using LegionMaster.Player.Squad.Config;
using LegionMaster.Player.Squad.Model;
using LegionMaster.Repository;
using SuperMaxim.Core.Extensions;
using UniRx;
using Zenject;

namespace LegionMaster.Player.Squad.Service
{
    public class PlayerSquadService
    {
        private readonly PlayerSquadConfig _playerSquadConfig;
        private readonly Dictionary<GameMode, ISingleModelRepository<SquadModel>> _repositories;
        private readonly Dictionary<GameMode, ReactiveProperty<SquadModel>> _squad;
        public IObservable<int> BattleModeUnitCount => GetSquad(GameMode.Battle).Select(it => it.Units.Count);
        public int MaxPlacedUnitCount => _playerSquadConfig.MaxPlacedUnitCount;
        
        public PlayerSquadService(PlayerSquadConfig playerSquadConfig, DiContainer container)
        {
            _playerSquadConfig = playerSquadConfig;

            _repositories = EnumExt.Values<GameMode>()
                                   .Select(type => (type, container.ResolveId<ISingleModelRepository<SquadModel>>(type)))
                                   .ToDictionary(pair => pair.type, pair => pair.Item2);

            _squad = EnumExt.Values<GameMode>()
                            .Select(type => (type, new ReactiveProperty<SquadModel>(GetSquadModel(type))))
                            .ToDictionary(pair => pair.type, pair => pair.Item2);
        }
        public void ResetSquad(GameMode gameMode)
        {
            _repositories[gameMode].Delete();
            _squad[gameMode].SetValueAndForceNotify(GetSquadModel(gameMode));
        }
        public void Set(SquadModel squadModel, GameMode gameMode)
        {
            _repositories[gameMode].Set(squadModel);
            _squad[gameMode].SetValueAndForceNotify(squadModel);
        }
        private SquadModel GetSquadModel(GameMode gameMode) => _repositories[gameMode].Get() ?? new SquadModel();

        public IReadOnlyReactiveProperty<SquadModel> GetSquad(GameMode gameMode) => _squad[gameMode];

        public void ResetAllSquads()
        {
            _squad.Keys.ForEach(ResetSquad);
        }
    }
}