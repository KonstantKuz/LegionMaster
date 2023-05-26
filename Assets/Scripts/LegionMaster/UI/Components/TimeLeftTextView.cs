using System;
using LegionMaster.Extension;
using LegionMaster.Localization.Service;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimeLeftTextView : MonoBehaviour
    {
        [SerializeField] private string _format;
        [Inject] private LocalizationService _localization;
        
        private TextMeshProUGUI _text;
        private IDisposable _disposable;
        private DateTime _time;
        private bool _useTimePostfix;
        public TextMeshProUGUI Text
        {
            get
            {
                return _text ??= GetComponent<TextMeshProUGUI>();
            }
        }

        public void Init(DateTime time, bool useTimePostfix = true)
        {
            _useTimePostfix = useTimePostfix;
            _disposable?.Dispose();
            _time = time;
            _disposable = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Subscribe(_ => UpdateText());
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void UpdateText()
        {
            var timeLeft = _time - DateTime.Now;
            if (timeLeft < TimeSpan.Zero) timeLeft = TimeSpan.Zero;
            if (_useTimePostfix) {
                Text.text = string.Format(_localization.Get(_format), timeLeft.ToFormattedString(_localization));
                return;
            }
            Text.text = string.Format(_format, timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
        }
    }
}