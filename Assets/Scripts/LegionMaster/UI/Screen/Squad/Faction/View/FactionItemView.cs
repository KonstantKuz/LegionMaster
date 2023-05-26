using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Squad.Faction.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Squad.Faction.View
{
    public class FactionItemView : MonoBehaviour
    {
        private static readonly Color GRAYSCALE_COLOR = new Color(0.3f, 0.4f, 0.6f);
        
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _factionUnitCountLabel;
        [SerializeField] private ActionButton _actionButton;   
        [SerializeField] private RectTransform _footnotePosition;
        
        
        public void Init(FactionItemModel model)
        {
            _actionButton.Init(() => { model.OnClick?.Invoke(_footnotePosition); });
            Image = model.IconPath;
            FactionUnitCountLabel = model.FactionUnitCountText;
            if (!model.Activated) {
                SetGrayscaleImage();
            }
        }
        private void SetGrayscaleImage()
        {
            _image.color = GRAYSCALE_COLOR;
        }
        private string Image
        {
            set => _image.sprite = Resources.Load<Sprite>(value);
        }
        private string FactionUnitCountLabel
        {
            set => _factionUnitCountLabel.text = value;
        }
    }
}