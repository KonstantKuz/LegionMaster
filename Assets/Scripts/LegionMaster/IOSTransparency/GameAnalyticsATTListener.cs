using System;
using GameAnalyticsSDK;
using UniRx;
using UnityEngine;

namespace LegionMaster.IOSTransparency
{
    //callback of IGameAnalyticsATTListener are called NOT from main thread!!!
    public class GameAnalyticsATTListener : IGameAnalyticsATTListener, IATTListener
    {
        public event Action OnStatusReceived;

        public static GameAnalyticsATTListener Create() => new GameAnalyticsATTListener();
        
        public void Init()
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }

        public void GameAnalyticsATTListenerNotDetermined()
        {
            Debug.Log("GameAnalytics IDFA status:= NotDetermined");
            CallEvent();
        }

        private void CallEvent()
        {
            MainThreadDispatcher.Post((obj) => (obj as Action)?.Invoke(), OnStatusReceived);
        }

        public void GameAnalyticsATTListenerRestricted()
        {
            Debug.Log("GameAnalytics IDFA status:= Restricted");
            CallEvent();
        }

        public void GameAnalyticsATTListenerDenied()
        {
            Debug.Log("GameAnalytics IDFA status:= Denied");
            CallEvent();
        }

        public void GameAnalyticsATTListenerAuthorized()
        {
            Debug.Log("GameAnalytics IDFA status:= Authorized");
            CallEvent();
        }
    }
}