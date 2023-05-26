using System;
using System.Collections.Generic;
using LegionMaster.Extension;
using LegionMaster.UI.Screen.Squad.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Squad.View
{
    public class UnitButtonsView : MonoBehaviour
    {
        [SerializeField]
        private UnitButton _buttonPrefab;
        [SerializeField]
        private Transform _buttonRoot;

        private CompositeDisposable _disposable;
        public Transform ButtonRoot => _buttonRoot;
        
        [Inject]
        private DiContainer _container;

        public void Init(IEnumerable<IObservable<UnitButtonModel>> models)
        {
            RemoveAllButtons();

            _disposable = new CompositeDisposable();
            foreach (var model in models)
            {
                var button = _container.InstantiatePrefabForComponent<UnitButton>(_buttonPrefab, _buttonRoot);
                model.Subscribe(it => button.Init(it)).AddTo(_disposable);
            }
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
        private void OnDisable()
        {
            RemoveAllButtons();
        }

        private void RemoveAllButtons()
        {
            _disposable?.Dispose();
            _buttonRoot.DestroyAllChildren();
        }
    }
}