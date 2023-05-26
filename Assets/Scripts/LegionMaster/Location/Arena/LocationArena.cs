using System;
using LegionMaster.Campaign.Location;
using LegionMaster.Core.Mode;
using LegionMaster.Location.GridArena;
using LegionMaster.UI.Screen.CampaignSquad.DisplayCase;
using LegionMaster.UI.Screen.DuelSquad.DisplayCase;
using LegionMaster.UI.Screen.Squad.SquadSetup;
using LegionMaster.Util.SerializableDictionary;
using SuperMaxim.Core.Extensions;
using UnityEngine;

namespace LegionMaster.Location.Arena
{
    public class LocationArena : GameWorld
    {
        private GameMode? _currentMode;

        [SerializeField] private GameObject _spawnContainer;
        [SerializeField] private Transform _groundPlane;
        [SerializeField] private Transform _exitFromArena;
        [SerializeField] private ArenaRewardChest _rewardChest;    
        [SerializeField] private CampaignStageDoors _campaignStageDoors;
  

        [SerializeField] private SerializableDictionary<GameMode, ArenaGrid> _grids;
        [SerializeField] private SerializableDictionary<GameMode, PlayerSquadSetup> _playerSquadSetups;
        [SerializeField] private EnemySquadSetup _enemySquadSetup;
        [SerializeField] private DuelDisplayCase _duelDisplayCase;
        [SerializeField] private CampaignDisplayCase _campaignDisplayCase;

        public GameObject SpawnContainer => _spawnContainer;
        public Transform GroundPlane => _groundPlane;
        public Transform ExitFromArena => _exitFromArena;
        public ArenaRewardChest RewardChest => _rewardChest;
        public CampaignStageDoors CampaignStageDoors => _campaignStageDoors;
        public PlayerSquadSetup CurrentPlayerSquadSetup => _playerSquadSetups[CurrentMode];
        public ArenaGrid CurrentGrid => _grids[CurrentMode];
        public float CellSize => CurrentGrid.CellSize;
        
        public DuelDisplayCase DuelDisplayCase => _duelDisplayCase;    
        public CampaignDisplayCase CampaignDisplayCase => _campaignDisplayCase;
        
        public GameMode CurrentMode
        {
            get
            {
                if (!_currentMode.HasValue) {
                    throw new NullReferenceException("GameMode is not set, please, set GameMode to Arena");
                }
                return _currentMode.Value;
            }
            set
            {
                _currentMode = value;
                SwitchGrid(value);
            }
        }
        
        public void ShowSquadSetups(bool showEnemy = true)
        {
            if (showEnemy && _enemySquadSetup.CanShow(CurrentMode)) {
                _enemySquadSetup.Show();
            }
            CurrentPlayerSquadSetup.Show();
        }
        
        public void HideSquadSetups()
        {
            CurrentPlayerSquadSetup.Hide();
            _enemySquadSetup.Hide();
        }
        public void ShowScene()
        {
            gameObject.SetActive(true);
        }
        public void HideScene()
        {
            gameObject.SetActive(false);
        }
        
        private void SwitchGrid(GameMode newMode)
        {
            _grids.ForEach(it => it.Value.gameObject.SetActive(false));
            _grids[newMode].gameObject.SetActive(true);
        }
    }
}