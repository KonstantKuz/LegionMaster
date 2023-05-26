using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Components
{
    public class SpriteView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void Init(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}