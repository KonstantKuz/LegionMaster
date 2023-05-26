using LegionMaster.Localization;
using UnityEngine;

namespace LegionMaster.UI.Dialog.Footnote.Model
{
    public class FootnoteInitModel
    {
        public Vector2 Position { get; }
        public Side Side { get; }
        public LocalizableText HeaderText { get; }
        public LocalizableText DescriptionText { get; }

        public FootnoteInitModel(Vector2 position, Side side, LocalizableText headerText, LocalizableText descriptionText)
        {
            Position = position;
            Side = side;
            HeaderText = headerText;
            DescriptionText = descriptionText;
        }

        public static FootnoteInitModel Create(RectTransform position, Side side, LocalizableText headerText, LocalizableText descriptionText)
        {
            return new FootnoteInitModel(position.position, side, headerText, descriptionText);
        }
    }
}
