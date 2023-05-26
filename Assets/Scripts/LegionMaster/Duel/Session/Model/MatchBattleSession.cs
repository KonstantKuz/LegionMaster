using System.Collections.Generic;
using System.Linq;
using LegionMaster.Core.Mode;
using LegionMaster.Duel.Config;
using LegionMaster.Enemy.Config;
using LegionMaster.Enemy.Service;
using LegionMaster.Extension;
using LegionMaster.Location.Session.Model;
using LegionMaster.Units.Component;

namespace LegionMaster.Duel.Session.Model
{
    public class DuelBattleSession : BattleSession
    {
        public readonly Dictionary<UnitType, int> WinRoundCount;
        public readonly int Match;

        public int Round;

        private DuelBattleSession(GameMode mode, EnemySquadConfig enemySquad, int match) : base(mode, enemySquad)
        {
            Match = match;
            Round = 1;
            WinRoundCount = EnumExt.Values<UnitType>().ToDictionary(type => type, type => 0);
        }

        public static DuelBattleSession Build(GameMode mode, EnemySquadService enemySquadService)
        {
            return new DuelBattleSession(mode, enemySquadService.GetMatchEnemySquad(mode, 1), enemySquadService.GetMatch(mode).Match);
        }

        public void Finish(UnitType winner)
        {
            SetWinnerByUnitType(winner);
        }

        public void IncreaseWinRoundCount(UnitType roundWinner)
        {
            WinRoundCount[roundWinner]++;
        }

        public UnitType? TryFindMatchWinner(DuelConfig duelConfig)
        {
            foreach (var score in WinRoundCount) {
                if (score.Value >= duelConfig.DuelWinCount) {
                    return score.Key;
                }
            }
            return null;
        }

        public void IncreaseRound(EnemySquadService enemySquadService)
        {
            ++Round;
            EnemySquad = enemySquadService.GetMatchEnemySquad(Mode, Round);
            EnemiesKilled = 0;
        }
    }
}