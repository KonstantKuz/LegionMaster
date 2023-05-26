using JetBrains.Annotations;
using UnityEngine;

namespace LegionMaster.ABTest
{
    [PublicAPI]
    public class ABTest
    {
        public const string TEST_ID = "version_0.12";

        private const string DEFAULT_VARIANT_ID = "default";
        private const string NO_UNLOCK_CHARACTER = "no_unlock_char";

        private string _testVariantId;

        public ABTest()
        {
            Reload();
        }

        public void Reload()
        {
            _testVariantId = PlayerPrefs.GetString(GetKey(TEST_ID), DEFAULT_VARIANT_ID);
            Debug.Log($"Setting ab test {TEST_ID} to {_testVariantId}");
        }
        public static void SetExperiment(string experimentId, string variantId)
        {
            PlayerPrefs.SetString(GetKey(experimentId), variantId);
        }
        private static string GetKey(string experimentId)
        {
            return $"ABTest_{experimentId}";
        }
        public bool NoUnlockCharacter => _testVariantId == NO_UNLOCK_CHARACTER;
    }
}