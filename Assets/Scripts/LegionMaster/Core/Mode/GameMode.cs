namespace LegionMaster.Core.Mode
{
    public enum GameMode
    {
        Battle, 
        Duel,
        Campaign,
        HyperCasual,
    }

    public static class GameModeExt
    { 
        public static bool IsMatchMode(this GameMode gameMode) => gameMode == GameMode.Duel || gameMode == GameMode.Campaign;
    }
}