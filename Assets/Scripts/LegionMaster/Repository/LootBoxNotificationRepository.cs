using LegionMaster.LootBox.Model;

namespace LegionMaster.Repository
{
    public class LootBoxNotificationRepository : LocalPrefsSingleRepository<LootBoxNotificationState>
    {
        protected LootBoxNotificationRepository() : base("lootBoxNotification")
        {
        }
    }
}