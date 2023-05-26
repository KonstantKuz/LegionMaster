using System;
using System.Collections.Generic;
using GameAnalyticsSDK;
using GameDevWare.Serialization;
using UnityEngine;

namespace LegionMaster.ABTest
{
    public class GameAnalyticsABTestProvider : IABTestProvider
    {
        private readonly ABTest _abTest;

        public event Action OnLoaded;

        public GameAnalyticsABTestProvider(ABTest abTest)
        {
            _abTest = abTest;
        }
        public void Load()
        {
            GameAnalytics.OnRemoteConfigsUpdatedEvent += SetAbConfig;
        }
        private void SetAbConfig()
        {
            var configString = GameAnalytics.GetRemoteConfigsContentAsString();
            Debug.Log($"GameAnalytics ab config callback is called with {configString}");
            string value = GameAnalytics.GetRemoteConfigsValueAsString(ABTest.TEST_ID);
            if (value != null) {
                Debug.Log($"Experiment {ABTest.TEST_ID} value is {value}");
                ABTest.SetExperiment(ABTest.TEST_ID, value);
            }
            _abTest.Reload();
            OnLoaded?.Invoke();
        }
    }
}