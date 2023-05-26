using EasyButtons;
using LegionMaster.Analytics;
using LegionMaster.Analytics.Data;
using LegionMaster.Extension;
using LegionMaster.Player.Progress.Service;
using LegionMaster.UI.Components;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Header
{
    public class PlayerPanelPresenter : MonoBehaviour
    {
        [Inject] private PlayerProgressService _progressService;
        [Inject] private Analytics.Analytics _analytics;
        
        [SerializeField] private TextMeshProLocalization _levelView;
        [SerializeField] private ProgressBarView _xpView;
        [SerializeField] private RectTransform _droppingLootContainer;
        
        private CompositeDisposable _disposable;
        private PlayerPanelModel _model;
        

        private void OnEnable()
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _model = new PlayerPanelModel(_progressService);
            _model.Level.Subscribe(it => _levelView.SetTextFormatted("lvl. {0}", it))
                .AddTo(_disposable);
            _model.LevelProgress.Subscribe(it => { _xpView.SetValueWithLoop(it); })
                .AddTo(_disposable);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
            _model = null;
        }

        [Button]
        public void AddExp(int amount)
        {
            using (_analytics.SetAcquisitionProperties("UnityEditor.AddExp", ResourceAcquisitionType.Boost))
            {
                _progressService.AddExp(amount);
            }
          
        }
    }
}