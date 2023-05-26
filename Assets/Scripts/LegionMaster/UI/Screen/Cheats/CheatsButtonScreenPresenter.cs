using LegionMaster.Cheats;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.UI.Screen.Cheats
{
    public class CheatsButtonScreenPresenter : MonoBehaviour
    {
        [Inject] private CheatsActivator _cheatsActivator;
        
        [SerializeField] private Button _cheatsButton;

        private void OnEnable() => _cheatsButton.onClick.AddListener(ShowCheatsPanel);

        private void OnDisable() => _cheatsButton.onClick.RemoveAllListeners();

        private void ShowCheatsPanel() => _cheatsActivator.ShowCheatsPanelScreen(true);
    }
}