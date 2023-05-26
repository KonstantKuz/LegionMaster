using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog.Footnote.Model;
using LegionMaster.UI.Dialog.Footnote.View;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Dialog.Footnote
{
    public class FootnoteDialogPresenter : BaseDialog, IUiInitializable<FootnoteInitModel>
    {
        [SerializeField] private FootnoteView _footnoteView;

        [Inject] private DialogManager _dialogManager;

        public void Init(FootnoteInitModel initModel)
        {
            _footnoteView.Init(initModel, OnCloseButton);
        }
        private void OnCloseButton()
        {
            _dialogManager.Hide<FootnoteDialogPresenter>();
        }
        
    }
}