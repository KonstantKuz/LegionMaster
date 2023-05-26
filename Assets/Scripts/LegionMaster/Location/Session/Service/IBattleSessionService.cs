using LegionMaster.Location.Session.Model;
using LegionMaster.Units.Component;

namespace LegionMaster.Location.Session.Service
{
    public interface IBattleSessionService
    {
        void StartBattle();
        BattleSession FinishBattle();
        void FinishBattleWithCheats(UnitType winner);
        BattleSession BattleSession { get; }  

    }
}