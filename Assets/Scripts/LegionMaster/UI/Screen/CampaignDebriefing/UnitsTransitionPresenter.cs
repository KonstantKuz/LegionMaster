using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using LegionMaster.Campaign.Session.Service;
using LegionMaster.Extension;
using LegionMaster.Location.Arena;
using LegionMaster.Location.Camera;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Campaign;
using LegionMaster.Units;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.CampaignDebriefing
{
    public class UnitsTransitionPresenter : BaseScreen
    {
        private const ScreenId UNITS_TRANSITION_SCREEN = ScreenId.UnitsTransition;
        public static readonly string URL = CampaignScreen.CAMPAIGN_SCREEN + "/" + UNITS_TRANSITION_SCREEN;
        public override ScreenId ScreenId => UNITS_TRANSITION_SCREEN; 
        public override string Url => URL;
        
        [SerializeField] private float _unitsSpeed = 15;
        [SerializeField] private float _overlayFadeDuration = 1;
        [SerializeField] private float _finishCameraMoveTime;
        [SerializeField] private float _chestOpenTime;
        [SerializeField] private Image _overlayBackground;
        
        private Coroutine _currentTransition;
        private Action _onTransitionCompleted;

        [Inject] private UnitsTransitionService _unitsTransitionService;
        [Inject] private LocationArena _locationArena;
        [Inject] private CameraManager _cameraManager;

        protected override void Awake()
        {
            base.Awake();
            InitOverlayBackground();
        }

        private void InitOverlayBackground()
        {
            _overlayBackground.transform.SetParent(transform.parent);
            _overlayBackground.transform.SetAsLastSibling();
            _overlayBackground.gameObject.SetActive(true);
        }

        [PublicAPI]
        public void Init(Action onTransitionCompleted, bool finishedMatch)
        {
            var transitionUnits = _unitsTransitionService.BuildTransitionUnits();
            if (!transitionUnits.Any())
            {
                Debug.LogWarning("There is no available units for transition. Force complete units transition.");
                onTransitionCompleted?.Invoke();
                return;
            }
            StopCurrentTransition();
            _onTransitionCompleted = onTransitionCompleted;
            _locationArena.CampaignStageDoors.Open();
            
            var transition = finishedMatch ? PlayTransitionToChest(transitionUnits) : PlayTransitionToExit(transitionUnits);
            _currentTransition = StartCoroutine(transition);
        }

        private void StopCurrentTransition()
        {
            if (_currentTransition == null)
            {
                return;
            }
            
            StopCoroutine(_currentTransition);
            _currentTransition = null;
        }
        
        private IEnumerator PlayTransitionToExit(List<Unit> transitionUnits)
        {
            yield return _unitsTransitionService.MoveUnitsToExit(transitionUnits, _unitsSpeed);
            yield return FinishExitTransition();
            StopCurrentTransition();
        }

        private IEnumerator FinishExitTransition()
        {
            yield return _overlayBackground.DOFade(1, _overlayFadeDuration).WaitForCompletion();
            _onTransitionCompleted?.Invoke();
            yield return _overlayBackground.DOFade(0, _overlayFadeDuration).WaitForCompletion();
        }

        private IEnumerator PlayTransitionToChest(List<Unit> transitionUnits)
        {
            _cameraManager.SwitchTo(CameraManager.CAMPAIGN_FINISH, _finishCameraMoveTime);
            yield return _unitsTransitionService.MoveUnitsToChest(transitionUnits, _unitsSpeed);
            yield return OpenChest();
            StopCurrentTransition();
        }
        
        private IEnumerator OpenChest()
        {
            _locationArena.RewardChest.Open();
            yield return new WaitForSeconds(_chestOpenTime);
            _onTransitionCompleted?.Invoke();
        }
    }
}