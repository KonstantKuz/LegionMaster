namespace LegionMaster.UI.Screen.BattlePass.Model
{
    public class ExpProgressModel
    {
        public int CurrentExp { get; private set; }
        public int MaxExp { get; private set; }
        public float ExpProgress { get; private set; }
        public int Level { get; private set; }
        
        public static ExpProgressModel Create(int currentExp, int maxExp, int level)
        {
            return new ExpProgressModel {
                    CurrentExp = currentExp,
                    MaxExp = maxExp,
                    Level = level,
                    ExpProgress = 1.0f * currentExp / maxExp,
            };
        }
    }
}