using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Components
{
    public class TextView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        private CompositeDisposable _disposable;
        public void Init(IObservable<string> model)
        {
            Dispose();
            _disposable = new CompositeDisposable();
            model.Subscribe(SetText);
        }       
        public void Init(IObservable<int> model)
        {
            Dispose();
            _disposable = new CompositeDisposable();
            model.Subscribe(it => SetText(it.ToString()));
        }
        private void SetText(string value)
        {
            _text.text = value;
        }

        private void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnDisable()
        {
            Dispose();
        }
    }
}