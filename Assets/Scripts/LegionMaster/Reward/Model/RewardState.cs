using LegionMaster.Reward.Config;

namespace LegionMaster.Reward.Model
{
    public class RewardState
    {
        public string Id;
        public int RoundCounter;
        public bool TakenThisRound;

        public bool HasRounds => RoundCounter > 0;
        public RewardState(string id)
        {
            Id = id;
        }
        
        public void ResetCounter()
        {
            RoundCounter = 1;
            TakenThisRound = false;
        }
        public void IncrementCounter(RewardConfig rewardConfig)
        {
            RoundCounter++;
            if (TakenThisRound && RoundCounter > rewardConfig.Rounds) {
                ResetCounter();
            }

        }
        public void MarkAsTaken()
        {
            if (!HasRounds) {
                return;
            }
            TakenThisRound = true;
        }
    }
}