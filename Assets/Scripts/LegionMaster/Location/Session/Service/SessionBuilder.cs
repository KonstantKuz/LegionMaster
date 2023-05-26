using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Campaign.Session.Service;
using LegionMaster.Campaign.Store;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Config;
using LegionMaster.Duel.Session.Service;
using LegionMaster.Duel.Store;
using LegionMaster.Enemy.Service;
using LegionMaster.HyperCasual.Store.Config;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Units.Component;
using Zenject;

namespace LegionMaster.Location.Session.Service
{
    [PublicAPI]
    public class SessionBuilder
    {
        private static Dictionary<GameMode, SessionParams> _sessions = new Dictionary<GameMode, SessionParams>() {
                {GameMode.Battle, new SessionParams(typeof(BattleSessionService), new object[] {GameMode.Battle})},
                {GameMode.Duel, new SessionParams(typeof(DuelSessionService))},
                {GameMode.Campaign, new SessionParams(typeof(CampaignSessionService))},
                {GameMode.HyperCasual, new SessionParams(typeof(BattleSessionService), new object[] {GameMode.HyperCasual})},
        };
        [Inject]
        private EnemySquadService _enemySquadService;
        [Inject]
        private DiContainer _container;
        [Inject]
        private DuelUnitStoreService _duelUnitStoreService;
        [Inject]
        private DuelConfig _duelConfig;
        [Inject]
        private WalletService _walletService;
        [Inject]
        private Analytics.Analytics _analytics;
        [Inject]
        private CampaignUnitStoreService _campaignUnitStoreService;
        [Inject]
        private PlayerProgressService _playerProgressService;
        [Inject]
        private BattleSessionServiceWrapper _battleSessionServiceWrapper;

        public void CreateDuel()
        {
            _duelUnitStoreService.Init();
            CreateSession(GameMode.Duel);
            _walletService.ResetCurrency(Currency.DuelToken);
            _walletService.Add(Currency.DuelToken, _duelConfig.TokensStartAmount);
            _analytics.ReportDuelStart(_playerProgressService.Progress.DuelProgress);
        }

        public void CreateCampaign()
        {
            _campaignUnitStoreService.Init();
            CreateSession(GameMode.Campaign);
            _analytics.ReportCampaignStart(_playerProgressService.Progress.CampaignProgress);
        }

        public void CreateBattle()
        {
            if (!_enemySquadService.BattleSquadExists) {
                return;
            }
            CreateSession(GameMode.Battle);
        }
        public void CreateHyperCasual()
        {
            CreateSession(GameMode.HyperCasual);
        }

        public void CreateNextDuelRound(UnitType winner)
        {
            var duelSessionService = _battleSessionServiceWrapper.GetImpl<DuelSessionService>();
            duelSessionService.IncreaseRound();
            _walletService.Add(Currency.DuelToken, _duelConfig.TokensPerRoundAmount);
        }

        public void CreateNextCampaignStage()
        {
            var campaignSessionService = _battleSessionServiceWrapper.GetImpl<CampaignSessionService>();
            campaignSessionService.IncreaseStage();
        }

        private void CreateSession(GameMode gameMode)
        {
            _battleSessionServiceWrapper.ChangeImpl((IBattleSessionService) _container.Instantiate(_sessions[gameMode].SessionType,
                                                                                                   _sessions[gameMode].Params));
        }
    }
}