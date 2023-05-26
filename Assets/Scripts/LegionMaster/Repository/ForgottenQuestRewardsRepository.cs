using JetBrains.Annotations;
using LegionMaster.Quest.Model;

namespace LegionMaster.Repository
{
    [PublicAPI]
    public class ForgottenQuestRewardsRepository: LocalPrefsSingleRepository<QuestRewardList>
    {
        protected ForgottenQuestRewardsRepository() : base("quest_rewards_v2") 
        {
        }
    }
}