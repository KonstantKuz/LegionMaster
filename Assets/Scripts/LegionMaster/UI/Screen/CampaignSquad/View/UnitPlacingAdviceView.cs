using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.CampaignSquad.Model;
using UniRx;
using UnityEngine;

namespace LegionMaster.UI.Screen.CampaignSquad.View
{
    public class UnitPlacingAdviceView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProLocalization _adviceText;
        [SerializeField]
        private GameObject _adviceContainer;
        private CompositeDisposable _disposable;

        public void Init(IReactiveProperty<UnitPlacingAdviceModel> advice)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();
            advice.Subscribe(InitAdviceText).AddTo(_disposable);
        }

        private void InitAdviceText(UnitPlacingAdviceModel advice)
        {
            _adviceContainer.SetActive(advice.Enabled);
            _adviceText.SetTextFormatted(_adviceText.LocalizationId, new object[] {advice.AlreadyPlacedUnitCount, advice.AvailableUnitCount});
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}