using LegionMaster.Campaign.Session.Messages;
using LegionMaster.Duel.Session.Messages;
using LegionMaster.Location.Session.Messages;
using LegionMaster.LootBox.Message;
using LegionMaster.Player.Inventory.Message;
using LegionMaster.Player.Inventory.Model;
using LegionMaster.UpgradeUnit.Message;
using SuperMaxim.Messaging;

namespace LegionMaster.Quest.Service
{
    public class QuestMessageHandler
    {
        private const string KILL_ENEMY_CONDITION = "KillEnemy";
        private const string PLAY_BATTLE_CONDITION = "PlayBattle";
        private const string WIN_BATTLE_CONDITION = "WinBattle";
        private const string UPGRADE_HERO_CONDITION = "UpgradeHero";
        private const string GET_SOFT_CONDITION = "GetSoft";
        private const string SPEND_HARD_CONDITION = "SpendHard";
        private const string OPEN_LOOTBOX_CONDITION = "OpenLootbox";
        
        private readonly QuestService _questService;
        
        public QuestMessageHandler(IMessenger messenger, QuestService questService)
        {
            _questService = questService;
            messenger.Subscribe<BattleEndMessage>(OnBattleFinished); 
            
            messenger.Subscribe<DuelBattleEndMessage>(OnDuelBattleFinished);        
            messenger.Subscribe<RoundEndMessage>(OnDuelRoundFinished);
            
            messenger.Subscribe<CampaignBattleEndMessage>(OnCampaignBattleFinished);        
            messenger.Subscribe<StageEndMessage>(OnCampaignStageFinished);
            
            messenger.Subscribe<UnitUpgradeMessage>(OnUnitUpgraded);
            messenger.Subscribe<CurrencyChangedMessage>(OnCurrencyChanged);
            messenger.Subscribe<LootboxOpenMessage>(OnLootboxOpened);
        }

        private void OnCurrencyChanged(CurrencyChangedMessage msg)
        {
            if (msg.Currency == Currency.Soft && msg.Delta > 0)
            {
                _questService.IncreaseCounter(GET_SOFT_CONDITION, msg.Delta);
            }

            if (msg.Currency == Currency.Hard && msg.Delta < 0)
            {
                _questService.IncreaseCounter(SPEND_HARD_CONDITION, -msg.Delta);
            }
        }

        private void OnUnitUpgraded(UnitUpgradeMessage msg)
        {
            _questService.IncreaseCounter(UPGRADE_HERO_CONDITION);
        }

        private void OnBattleFinished(BattleEndMessage msg)
        {
            _questService.IncreaseCounter(KILL_ENEMY_CONDITION, msg.EnemiesKilled);
            IncreasePlayBattleCounter(msg.IsPlayerWon);
        }  
        private void OnCampaignBattleFinished(CampaignBattleEndMessage msg)
        {
            IncreasePlayBattleCounter(msg.IsPlayerWon);
        } 
        private void OnDuelBattleFinished(DuelBattleEndMessage msg)
        {
            IncreasePlayBattleCounter(msg.IsPlayerWon);
        }

        private void IncreasePlayBattleCounter(bool isPlayerWon)
        {
            _questService.IncreaseCounter(PLAY_BATTLE_CONDITION);
            if (isPlayerWon) {
                _questService.IncreaseCounter(WIN_BATTLE_CONDITION);
            }
        }

        private void OnDuelRoundFinished(RoundEndMessage msg)
        {
            _questService.IncreaseCounter(KILL_ENEMY_CONDITION, msg.EnemiesKilled);
        }  
        private void OnCampaignStageFinished(StageEndMessage msg)
        {
            _questService.IncreaseCounter(KILL_ENEMY_CONDITION, msg.EnemiesKilled);
        }
        private void OnLootboxOpened(LootboxOpenMessage msg)
        {
            _questService.IncreaseCounter(OPEN_LOOTBOX_CONDITION, msg.Count);
        }
    }
}