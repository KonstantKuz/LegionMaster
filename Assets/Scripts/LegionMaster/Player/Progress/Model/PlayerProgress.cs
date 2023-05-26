using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.Config;
using LegionMaster.Core.Mode;
using LegionMaster.Extension;
using LegionMaster.Player.Progress.Config;

namespace LegionMaster.Player.Progress.Model
{
    public class PlayerProgress
    {
        public int Level = 1;
        public int Exp;

        public Dictionary<GameMode, BattleCountInfo> BattleCountInfos;
        public Dictionary<PlayerCollectibles, int> Collectibles;
        
        public int BattleProgress => BattleCountInfos[GameMode.Battle].WinCount; 
        public int DuelProgress => BattleCountInfos[GameMode.Duel].Count;
        public int CampaignProgress => BattleCountInfos[GameMode.Campaign].WinCount;
        public int HyperCasualProgress => BattleCountInfos[GameMode.HyperCasual].WinCount;
        public int TotalProgress => DuelProgress;
        public PlayerProgress()
        {
            BattleCountInfos = EnumExt.Values<GameMode>()
                                      .ToDictionary(mode => mode, mode => new BattleCountInfo());
        }
        
        public void IncreaseBattleCount(GameMode mode)
        {
            BattleCountInfos[mode].Count++;
        } 
        public void IncreaseBattleWinCount(GameMode mode)
        {
            BattleCountInfos[mode].WinCount++; 
        }
        public int MaxPlayerLevel(StringKeyedConfigCollection<LevelConfig> levels) => levels.Values.Count;

        public bool IsMaxLevel(StringKeyedConfigCollection<LevelConfig> levels) => Level >= levels.Values.Count + 1;

        public int MaxExpForCurrentLevel(StringKeyedConfigCollection<LevelConfig> levels) => CurrentLevelConfig(levels).ExpToNextLevel;

        public LevelConfig CurrentLevelConfig(StringKeyedConfigCollection<LevelConfig> levels) =>
                levels.Values[Math.Min(levels.Values.Count - 1, Level - 1)];

        public void AddExp(int amount, StringKeyedConfigCollection<LevelConfig> levels)
        {
            Exp += amount;
            while (Exp >= MaxExpForCurrentLevel(levels) && !IsMaxLevel(levels))
            {
                Exp -= MaxExpForCurrentLevel(levels);                
                Level++;
            }
        }

        public void IncreasePlayerCollectible(PlayerCollectibles collectible, int count = 1)
        {
            Collectibles ??= new Dictionary<PlayerCollectibles, int>();

            if (!Collectibles.ContainsKey(collectible))
            {
                Collectibles[collectible] = 0;
            }
            
            Collectibles[collectible]+=count;
        }
    }
}