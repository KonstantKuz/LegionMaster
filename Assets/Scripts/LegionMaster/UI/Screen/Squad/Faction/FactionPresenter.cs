using System.Linq;
using LegionMaster.Core.Config;
using LegionMaster.Factions.Config;
using LegionMaster.Localization;
using LegionMaster.Player.Inventory.Service;
using LegionMaster.Player.Squad.Service;
using LegionMaster.UI.Dialog;
using LegionMaster.UI.Dialog.Footnote;
using LegionMaster.UI.Dialog.Footnote.Model;
using LegionMaster.UI.Screen.Squad.Faction.Model;
using LegionMaster.UI.Screen.Squad.Faction.View;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Squad.Faction
{
    public class FactionPresenter : MonoBehaviour
    {
        public const string DESCRIPTION_LOCALIZATION_POSTFIX = "Description";

        [SerializeField] private FactionView _factionView;      
        [SerializeField] private Side _side;

        [Inject] private PlayerSquadService _squadService;
        [Inject] private InventoryService _inventoryService;
        [Inject] private DialogManager _dialogManager;
        [Inject] private FactionConfigCollection _factionConfig;
        private FactionModel _model;

        private void OnEnable()
        {
            _model = new FactionModel(_squadService, _inventoryService,
                                      (id, pos) => ShowFactionFootnote(id, pos, _side, _factionConfig, _dialogManager));
            _factionView.Init(_model.Factions);
        }

        private void OnDisable()
        {
            _model?.Dispose();
            _model = null;
        }

        public static void ShowFactionFootnote(string factionId, 
                                               RectTransform position, 
                                               Side side, 
                                               FactionConfigCollection factionConfig, 
                                               DialogManager dialogManager)
        {
            var localizationArgs = factionConfig.GetFactionConfig(factionId).Modifiers.Select(it => it.Value).Cast<object>().ToArray();
            var descriptionText = LocalizableText.Create($"{factionId}{DESCRIPTION_LOCALIZATION_POSTFIX}", localizationArgs);
            var initModel = FootnoteInitModel.Create(position, side, LocalizableText.Create(factionId), descriptionText);
            dialogManager.Show<FootnoteDialogPresenter, FootnoteInitModel>(initModel);
        }
    }
}