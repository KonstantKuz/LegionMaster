using LegionMaster.Localization;
using LegionMaster.UI.Components;
using LegionMaster.UI.Dialog.Footnote.Model;
using UnityEngine;

namespace LegionMaster.UI.Dialog.Footnote.View
{
    public class FootnoteDescriptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProLocalization _headerLabel;
        [SerializeField] private TextMeshProLocalization _descriptionLabel;  
        [SerializeField] private RectTransform _backgroundRect;
        [SerializeField] private Vector3 _leftBackgroundRotation = new Vector3(180, 180, 0);
        [SerializeField] private Vector3 _rightBackgroundRotation = new Vector3(180, 0, 0);
        
        private RectTransform _rectTransform;

        public void Init(FootnoteInitModel model)
        {
            HeaderLabel = model.HeaderText;
            DescriptionLabel = model.DescriptionText;
            SetSide(model.Side);
            RectTransform.position = model.Position;
        }

        private void SetSide(Side side)
        {
            switch (side) {
                case Side.LeftTop:
                    RectTransform.pivot = new Vector2(0, RectTransform.pivot.y);
                    _backgroundRect.rotation = Quaternion.Euler(_leftBackgroundRotation);
                    break;
                case Side.RightTop:
                    RectTransform.pivot = new Vector2(1, RectTransform.pivot.y);
                    _backgroundRect.rotation = Quaternion.Euler(_rightBackgroundRotation);
                    break;
            }
        }
        private LocalizableText HeaderLabel
        {
            set => _headerLabel.SetTextFormatted(value);
        }   
        private LocalizableText DescriptionLabel
        {
            set => _descriptionLabel.SetTextFormatted(value);
        }
        private RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();
    }
}