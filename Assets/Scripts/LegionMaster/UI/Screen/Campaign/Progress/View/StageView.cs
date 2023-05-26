using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Campaign.Progress.View
{
    public class StageView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pointerImage;
        [SerializeField]
        private Image _image;
        
        public void Init(Sprite stageSprite, bool lastStage)
        {
            _pointerImage.SetActive(lastStage);
            _image.sprite = stageSprite;
        }
    }
}