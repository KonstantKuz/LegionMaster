using LegionMaster.Campaign.Session.Model;

namespace LegionMaster.UI.Screen.Campaign.Progress.Model
{
    public class MatchProgressModel
    {
        public readonly int StageCount;
        public readonly int CurrentStage;
        
        public MatchProgressModel(int stageCount, int currentStage)
        {
            StageCount = stageCount;
            CurrentStage = currentStage;
        }
        public static MatchProgressModel CreateForCampaign(CampaignBattleSession session) =>
                new MatchProgressModel(session.LastStageNumber, session.Stage);
    }
}