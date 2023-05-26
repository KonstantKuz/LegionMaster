using LegionMaster.BattlePass.Service;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.BattlePass.Model;
using LegionMaster.UI.Screen.BattlePass.View;
using UniRx;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.BattlePass.Widget
{
    public class BattlePassWidgetPresenter : MonoBehaviour
    {
        [SerializeField] private ExpProgressView _expProgressView;
        [SerializeField] private ActionButton _toBattleBassButton;

        [Inject] private BattlePassService _battlePassService;
        [Inject] private BattlePassShowChecker _battlePassShowChecker;

        private CompositeDisposable _disposable;
        
        private void OnEnable()
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            _toBattleBassButton.Init(ShowBattlePass);
            UpdateExpProgress();
            var battlePassState = _battlePassService.StateChanged.Publish();
            battlePassState.Connect().AddTo(_disposable);
            battlePassState.Subscribe(_ => { UpdateExpProgress(); }).AddTo(_disposable);
        }

        private void UpdateExpProgress() => _expProgressView.Init(BuildExpProgressModel());

        private ExpProgressModel BuildExpProgressModel()
        {
            var currentExp = _battlePassService.Exp.Value;
            var maxExp = _battlePassService.MaxExpForCurrentLevel;
            return ExpProgressModel.Create(currentExp, maxExp, _battlePassService.Level.Value);
        }

        private void ShowBattlePass() => _battlePassShowChecker.ShowBattlePass(true);

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}