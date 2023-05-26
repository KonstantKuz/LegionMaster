using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LegionMaster.Location.Camera
{
    public class CameraManager : MonoBehaviour
    {
        public const string BATTLE_MODE = "battleMode";   
        public const string BATTLE_SQUAD_MODE = "battleSquadMode";    
        public const string DUEL_SQUAD_MODE = "duelSquad";    
        public const string CAMPAIGN_SQUAD_MODE = "сampaignSquad";
        public const string BATTLE_PLAY_MODE = "battle";      
        public const string CAMPAIGN_PLACING_SQUAD = "сampaignPlacingSquad";
        public const string CAMPAIGN_FINISH = "campaignFinish";
        
        [SerializeField] 
        private Transform _cameraRoot;
        [SerializeField] 
        private List<GameObject> _positions;
        
        private Sequence _tweenSequence;
        private string _currentMode;
        public void SwitchTo(string mode, float switchTime)
        {
            StopAnimation();
            _currentMode = mode;
            var targetTransform = GetPosition(mode);
            _tweenSequence = DOTween.Sequence()
                                    .Join(_cameraRoot.DOMove(targetTransform.position, switchTime))
                                    .Join(_cameraRoot.DORotate(targetTransform.rotation.eulerAngles, switchTime));
        }

        public void SwitchToImmediately(string mode)
        {
            _currentMode = mode;
            var targetTransform = GetPosition(mode);
            _cameraRoot.SetPositionAndRotation(targetTransform.position, targetTransform.rotation); 
        }
        private void OnDisable()
        {
            StopAnimation();
        }
        private void StopAnimation()
        {
            if (_tweenSequence.IsActive() && _tweenSequence.IsPlaying()) {
                _tweenSequence.Kill();
            }
            if (_currentMode != null) {
                SwitchToImmediately(_currentMode);
            }
        }
        private Transform GetPosition(string mode)
        {
            return _positions.Find(it => it.name == mode).transform;
        }
    }
}