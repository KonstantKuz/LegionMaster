using System;
using LegionMaster.UI.Screen.BattlePass.Model;
using TMPro;
using UnityEngine;
using static LegionMaster.UI.Screen.BattlePass.Model.LevelInfoModel;

namespace LegionMaster.UI.Screen.BattlePass.View
{
    public class BattlePassLevelInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private GameObject _availableImage;

        public void Init(LevelInfoModel levelInfo)
        {
            Level = levelInfo.Level.ToString();
            SetAvailableState(levelInfo.Available);
        }

        private void SetAvailableState(bool available)
        {
            _availableImage.SetActive(available);
        }

        private string Level
        {
            set => _level.text = value;
        }
    }
}