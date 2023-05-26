using System;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Description.Model;
using TMPro;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelSquad.View
{
    public class RoundInfoView : MonoBehaviour
    {
        [SerializeField] private TimeLeftTextView _timerView;      
        [SerializeField] private GameObject _timerContainer;  
        [SerializeField] private TextMeshProUGUI _playerScore;      
        [SerializeField] private TextMeshProUGUI _enemyScore; 
        [SerializeField] private TextMeshProLocalization _roundText;

        private Transform _timerOriginParent;
        private CompositeDisposable _disposable;
        public void Init(IObservable<RoundInfoModel> model)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            model.Subscribe(InitRoundInfo).AddTo(_disposable);
        }
        private void InitRoundInfo(RoundInfoModel model)
        {
            _timerView.Init(model.RoundStartTime, false);
            _timerContainer.SetActive(model.TimerEnabled);
            _playerScore.text = model.PlayerScore;
            _enemyScore.text = model.EnemyScore;
            _roundText.SetTextFormatted(_roundText.LocalizationId, model.RoundNumber);
            _timerOriginParent = _timerContainer.transform.parent;
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void PrepareToHighlight(Transform timerHighlight, int secondsToShow)
        {
            _timerContainer.SetActive(true);
            _timerContainer.transform.SetParent(timerHighlight);
            _timerView.Text.SetText(secondsToShow.ToString());
        }

        public void ResetFromHighlight()
        {
            _timerContainer.transform.SetParent(_timerOriginParent);
        }
    }
}