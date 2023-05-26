using UnityEngine;

namespace LegionMaster.Analytics.Wrapper.GameAnalytics
{
    public static class GameAnalyticsId
    {
        private const string NEW_ID_FLAG = "GameAnalyticsNewId";

        public static void GenerateNewIdOnNextStart()
        {
            PlayerPrefs.SetInt(NEW_ID_FLAG, 1);
        }

        public static bool ShouldGenerateNewId()
        {
            return PlayerPrefs.GetInt(NEW_ID_FLAG) == 1;
        }

        public static void ClearFlag()
        {
            PlayerPrefs.DeleteKey(NEW_ID_FLAG);
        }
    }
}