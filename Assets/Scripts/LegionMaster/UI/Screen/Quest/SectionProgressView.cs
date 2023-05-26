using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Quest
{
    public class SectionProgressView : MonoBehaviour
    {
        [SerializeField] private Image _progressBar;
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private SectionRewardIconView[] _rewards;

        public void Init(SectionProgressModel model)
        {
            _progressBar.fillAmount = model.Progress;
            _expText.text = model.Exp.ToString();
            for (int i = 0; i < _rewards.Length; i++)
            {
                _rewards[i].Init(model.Rewards[i]);
            }
        }
    }
}