using System;
using LegionMaster.BattlePass.Config;

namespace LegionMaster.BattlePass.Model
{
    public class BattlePassProgress
    {
        private const int INITIAL_EXP = 0;       
        private const int INITIAL_LEVEL = 1;
        
        public int Exp;
        public int Level;
        public bool PremiumActive;

        public BattlePassProgress()
        {
            Exp = INITIAL_EXP;
            Level = INITIAL_LEVEL;
            PremiumActive = false;
        }
        public bool IsMaxLevel(BattlePassConfigList config) => Level >= config.GetMaxLevelConfig().Level;
        
        public int MaxExpForCurrentLevel(BattlePassConfigList config) => CurrentLevelConfig(config).ExpToNextLevel;

        public int GetNeedExpUntilNextLevel(BattlePassConfigList config) => Math.Max(0, MaxExpForCurrentLevel(config) - Exp);

        public void AddExp(int amount, BattlePassConfigList config)
        {
            Exp += amount;
            while (Exp >= MaxExpForCurrentLevel(config))
            {
                if (IsMaxLevel(config)) {
                    Exp = MaxExpForCurrentLevel(config);
                    return;
                }
                Exp -= MaxExpForCurrentLevel(config);                
                Level++;
            }
        }
        
        private BattlePassConfig CurrentLevelConfig(BattlePassConfigList config)
        {
            return config.GetConfigByLevel(Level);
        }
    }
}