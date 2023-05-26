using System.Collections.Generic;

namespace LegionMaster.Tutorial.Model
{
    public enum TutorialScenarioId
    {
        SquadScreen,
        Test,
        Launch,
        LaunchCampaign,
        UnlockCharacter,
        SecondUnlockCharacter,
        LaunchHyperCasual,
    }
    
    public class TutorialState
    {
        public readonly Dictionary<TutorialScenarioId, bool> Scenarios = new Dictionary<TutorialScenarioId, bool>();
    }
}