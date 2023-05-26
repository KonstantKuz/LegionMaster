using LegionMaster.Player.Progress.Model;

namespace LegionMaster.Repository
{
    public class PlayerProgressRepository : LocalPrefsSingleRepository<PlayerProgress>
    {
        public const string PLAYER_PREFS_KEY = "PlayerProgress_v";
        private const int VERSION = 3;
        protected PlayerProgressRepository() : base(PLAYER_PREFS_KEY + VERSION)
        {
        }
    }
}