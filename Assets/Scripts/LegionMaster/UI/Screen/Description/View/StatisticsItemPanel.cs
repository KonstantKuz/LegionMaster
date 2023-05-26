using LegionMaster.UI.Components;
using TMPro;
using UnityEngine;

namespace LegionMaster.UI.Screen.Description.View
{
    public class StatisticsItemPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProLocalization _label;   
        [SerializeField]
        private TMP_Text _value;

        public void Init(string localizationId, string value = "")
        {
            _label.LocalizationId = localizationId;
            _value.text = value;
        }

        public void InitFormatted(string localizationId, object[] args)
        {
            _label.SetTextFormatted(localizationId, args);
        }

        public void SetTextColor(Color color)
        {
            _label.TextComponent.color = color;
        }
    }
}