using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LegionMaster.Core.Mode;
using LegionMaster.Player.Progress.Service;
using LegionMaster.Tutorial.UI;
using LegionMaster.UI;
using LegionMaster.UI.Screen.HyperCasualMode;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.Units.Component;
using UnityEngine;
using Zenject;
using Unit = LegionMaster.Units.Unit;

namespace LegionMaster.Tutorial.Scenario
{
    public class LaunchHyperCasual : TutorialScenario
    {
        [SerializeField] private float _dragTime = 1f;
        [SerializeField] private float _showUnitDelay = 0.5f;
        [SerializeField] private float _handShowPeriod = 0.5f;
        
        private HyperCasualModePresenter _hyperCasualPresenter;

        [Inject] private UIRoot _uiRoot;
        [Inject] private PlayerProgressService _playerProgress;

        private HyperCasualModePresenter HyperCasualModePresenter =>
            _hyperCasualPresenter ??= _uiRoot.gameObject.GetComponentInChildren<HyperCasualModePresenter>();
        private HyperCasualPlayerSquadSetup SquadSetup => HyperCasualModePresenter.SquadSetup; 
        
        public override bool IsStartAllowed(string screenUrl)
        {
            return screenUrl == HyperCasualModePresenter.URL && _playerProgress.GetWinBattleCount(GameMode.HyperCasual) <= 0;
        }

        public override IEnumerator Run()
        {
            FinishScenario();
            
            HighlightBuyMeleeUnitButton();
            yield return WaitForBuy2MeleeUnits();
            ClearHighlight();
            yield return WaitForUnitsMerged();
            yield return ClickUiElementStep("HyperCasualStartButton", HandDirection.Up, true, false);
            CompleteSteps();
        }

        private void HighlightBuyMeleeUnitButton()
        {
            var button = TutorialUiElementObserver.Get("BuyMeleeUnitButton");
            UiTools.TutorialHand.ShowOnElement(button, HandDirection.Down);
            UiTools.ElementHighlighter.Set(button, false);
        }

        private IEnumerator WaitForBuy2MeleeUnits()
        {
            while (SquadSetup.PlacedUnits.Count != 2)
            {
                yield return null;
            }
        }

        private void ClearHighlight()
        {
            UiTools.TutorialHand.Hide();
            UiTools.ElementHighlighter.Clear();
        }

        private IEnumerator WaitForUnitsMerged()
        {
            HyperCasualModePresenter.SetButtonsInteractable(false);
            var playerUnits = SquadSetup.PlacedUnits.Select(it => SquadSetup.GetUnitObject(it)).ToList();
            while (SquadSetup.PlacedUnits.Count == 2)
            {
                if (HyperCasualModePresenter.UnitCursor.IsUnitAttached)
                {
                    yield return null;
                    continue;
                }

                UiTools.TutorialHand.ShowPressingFingerOnObject(playerUnits[0].transform);
                yield return new WaitForSeconds(_showUnitDelay);
                yield return UiTools.TutorialHand.ShowDrag(playerUnits[0].transform, playerUnits[1].transform, _dragTime).WaitForCompletion();
                yield return new WaitForSeconds(_handShowPeriod);
            }
            HyperCasualModePresenter.SetButtonsInteractable(true);
        }
    }
}
