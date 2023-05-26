using System.Collections;
using System.Linq;
using LegionMaster.Localization.Service;
using LegionMaster.Location.Arena;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.Tutorial.Model;
using LegionMaster.Tutorial.UI;
using LegionMaster.UI.Screen.Squad;
using LegionMaster.UI.Screen.Squad.FreeCellProvider;
using LegionMaster.UI.Screen.Squad.View;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LegionMaster.Tutorial.ScreenTutorial
{
    public class SquadScreenTutorial : MonoBehaviour
    {
        private const string LOCALIZATION_TAP_TO_PLACE_UNIT = "Tap to place unit";
        private const string LOCALIZATION_DRAG_UNIT_HERE = "Drag unit here";
        private const string LOCALIZATION_START_BATTLE = "Start battle";
        private static readonly CellId DRAG_STEP_CELL_ID = new CellId(1, 0);

        [SerializeField]
        private UnitButtonsView _unitButtonsView;
        [SerializeField]
        private TutorialHand _tutorialHand;
        [SerializeField]
        private SquadPresenter _squadPresenter;
        [SerializeField]
        private RectTransform _toBattleButton;
        [SerializeField]
        private TMP_Text _tutorialText;
        [SerializeField]
        private CellId[] _unitPlacementCellIds = {new CellId(2, 0), new CellId(1, 0), new CellId(0, 3)};
        [Inject]
        private LocationArena _locationArena;
        [Inject]
        private LocalizationService _localization;
        [Inject(Optional = true)]
        private TutorialService _tutorialService;
        private void OnEnable()
        {
            _tutorialHand.Hide();
            HideText();
            if (!_tutorialService.IsScenarioCompleted(TutorialScenarioId.SquadScreen)) { // todo: move to tutorial service
                _tutorialService.CompleteScenario(TutorialScenarioId.SquadScreen);
                StartCoroutine(Run());
            }
        }

        private void ShowText(string localizationId)
        {
            _tutorialText.gameObject.SetActive(true);
            _tutorialText.text = _localization.Get(localizationId);
        }

        private void HideText()
        {
            _tutorialText.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _tutorialHand.Hide();
            HideText();
        }
        private IEnumerator Run()
        {
            yield return new WaitForUnitButtons(_unitButtonsView.ButtonRoot);

            _squadPresenter.SetFreeCellProvider(new FixedSequenceCellProvider(_squadPresenter.SquadSetup, _unitPlacementCellIds));

            var button = GetUnplacedUnitButton();
            var placedCount = 0;
            while (button != null && placedCount < 3) {
                ShowText(LOCALIZATION_TAP_TO_PLACE_UNIT);
                _tutorialHand.ShowOnElement(button.transform as RectTransform);
                yield return button.GetComponent<Button>().OnClickAsObservable().First().ToYieldInstruction();
                button = GetUnplacedUnitButton();
                placedCount++;
            }

            _squadPresenter.SetFreeCellProvider(_squadPresenter.DefaultFreeCellProvider);
            _tutorialHand.Hide();

            var targetCell = _locationArena.CurrentGrid.GetCell(DRAG_STEP_CELL_ID);
            _tutorialHand.ShowDrag(targetCell.transform);
            ShowText(LOCALIZATION_DRAG_UNIT_HERE);
            yield return _squadPresenter.StartUnitDragAsObservable.First().ToYieldInstruction();
            _tutorialHand.Hide();

            yield return _squadPresenter.StopUnitDragAsObservable.First().ToYieldInstruction();
            _tutorialHand.ShowOnElement(_toBattleButton);
            ShowText(LOCALIZATION_START_BATTLE);
        }

        private UnitButton GetUnplacedUnitButton()
        {
            return _unitButtonsView.ButtonRoot.GetComponentsInChildren<UnitButton>().FirstOrDefault(it => !it.IsUnitPlaced);
        }

        private class WaitForUnitButtons : CustomYieldInstruction
        {
            private readonly Transform _unitButtonRoot;

            public WaitForUnitButtons(Transform unitButtonRoot)
            {
                _unitButtonRoot = unitButtonRoot;
            }
            public override bool keepWaiting => _unitButtonRoot.childCount == 0;
        }
    }
}