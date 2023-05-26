using LegionMaster.Cheats;
using LegionMaster.UI.Components;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Cheats
{
    public class CodeInputScreenPresenter : MonoBehaviour
    {
        [Inject] private CheatsActivator _cheatsActivator;

        [SerializeField] private InputField _inputField;
        [SerializeField] private ActionButton _closeButton;
        [SerializeField] private ActionButton _confirmButton;

        private void OnEnable()
        {
            _closeButton.Init(HideCodeInputScreen);
            _confirmButton.Init(CheckInputCode);
        }

        private void CheckInputCode()
        {
            if (_cheatsActivator.IsValidInputCode(_inputField.text))
            {
                ShowCheatsButtonScreen();
                HideCodeInputScreen();
                EnableCheats();
            }
        }
        private void HideCodeInputScreen() => _cheatsActivator.ShowCodeInputScreen(false);
        private void EnableCheats() => _cheatsActivator.EnableCheats(true);
        private void ShowCheatsButtonScreen() => _cheatsActivator.ShowCheatsButtonScreen(true);
    }
}