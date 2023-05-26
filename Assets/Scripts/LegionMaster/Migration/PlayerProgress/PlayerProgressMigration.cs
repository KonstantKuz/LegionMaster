using LegionMaster.Core.Mode;
using LegionMaster.Repository;
using UnityEngine;

namespace LegionMaster.Migration.PlayerProgress
{
    public class PlayerProgressMigration
    {
        public static void TryMigrationToV3()
        {
            var prefsKeyV2 = PlayerProgressRepository.PLAYER_PREFS_KEY + "0.2.0";
            if (!PlayerPrefs.HasKey(prefsKeyV2)) {
                Debug.Log($"{prefsKeyV2} already migrated or new player");
                return;
            }
            var repoV2 = new LocalPrefsSingleRepository<PlayerProgressV2>(prefsKeyV2);
            var progressV2 = repoV2.Get();
            PlayerPrefs.DeleteKey(prefsKeyV2);
            if (progressV2 == null) {
                Debug.LogWarning($"Error deserialize {prefsKeyV2}, {prefsKeyV2} is null");
                return;
            }
            var repoV3 = new LocalPrefsSingleRepository<PlayerProgressV3>(PlayerProgressRepository.PLAYER_PREFS_KEY + 3);
            var progressV3 = repoV3.Get() ?? new PlayerProgressV3();
            progressV3.BattleCountInfos[GameMode.Battle].Count = progressV2.BattleCount;
            progressV3.BattleCountInfos[GameMode.Battle].WinCount = progressV2.BattleWinCount;
            progressV3.BattleCountInfos[GameMode.Duel].Count = progressV2.DuelCount;
            progressV3.BattleCountInfos[GameMode.Duel].WinCount = progressV2.DuelWinCount;
            repoV3.Set(progressV3);
        }
    }
}