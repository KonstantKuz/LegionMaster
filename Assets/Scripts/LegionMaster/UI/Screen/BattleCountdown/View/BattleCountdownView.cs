using System;
using System.Collections.Generic;
using LegionMaster.Localization.Service;
using LegionMaster.UI.Screen.BattleCountdown.Model;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.BattleCountdown.View
{
    public class BattleCountdownView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _countdownText;  
        [SerializeField]
        private float _timerPeriod = 1;
        [SerializeField]
        private List<Color> _countdownColors;
        [Inject]
        private LocalizationService _localization;

        private CompositeDisposable _disposable;
        private CountdownModel _countdownModel;

        private int _leftTime;

        public void Init(CountdownModel countdownModel, Action onStart)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _countdownModel = countdownModel;
            _leftTime = countdownModel.CountdownStartValue + 1;
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(_timerPeriod)).Subscribe(_ => UpdateCountdown(onStart)).AddTo(_disposable);
        }

        private void UpdateCountdown(Action onStart)
        {
            --_leftTime;
            if (_leftTime < 0) {
                onStart?.Invoke();
                Dispose();
                return;
            }
            var countdownText = _leftTime.ToString();
            if (_leftTime == 0) {
                countdownText = _localization.Get(_countdownModel.CountdownFinishText);
            }
            _countdownText.color = _countdownColors[_leftTime];
            _countdownText.text = countdownText;
        }

        private void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
            _countdownModel = null;
            _countdownText.text = "";
        }

        private void OnDisable()
        {
            Dispose();
        }
    }
}