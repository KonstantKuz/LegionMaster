using System;
using System.Collections.Generic;
using System.Linq;
using LegionMaster.BattlePass.Model;
using LegionMaster.Reward.Model;
using LegionMaster.Tutorial.UI;
using LegionMaster.UI.Screen.BattlePass.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.BattlePass.View
{
    [Serializable]
    public struct ViewStateContainer
    {
        public BattlePassRewardViewState State;
        public GameObject Container;
    }

    [RequireComponent(typeof(Button))]
    public class BattlePassRewardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rewardCount;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private Image _background;
        
        [SerializeField] private List<ViewStateContainer> _containers;  
        
        [SerializeField] private GameObject _rewardRoot;

        private Button _button;
        private Action _takeReward;
        private TutorialUiElement _tutorialUiElement;

        public void Init(BattlePassRewardModel model, Action takeReward)
        {
            if (model.ViewState == BattlePassRewardViewState.NoReward) {
                _rewardRoot.SetActive(false);
                return;
            }

            var reward = model.BattlePassReward.Reward;
            if (reward == null) {
                throw new Exception($"RewardItem is null, battlePassRewardId:= {model.BattlePassReward.Id.ToString()}, battlePassRewardState:= {model.BattlePassReward.State}");
            }
            Background = reward.GetBackgroundPath();
            RewardCount = reward.Count.ToString();
            RewardIcon = reward.GetIconPath();
            
            SetState(model);
            _takeReward = takeReward;

            TutorialUiElement.Id = GetTutorialId(model.BattlePassReward.Id);
        }
        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
        }
        public static string GetTutorialId(BattlePassRewardId rewardId)
        {
            return $"BattlePassReward_{rewardId}";
        }
        
        private void SetState(BattlePassRewardModel model)
        {
            Button.interactable = false;
            DisableAllContainers();
            
            EnableContainer(model.ViewState);
            if (model.ViewState == BattlePassRewardViewState.Available) {
                Button.interactable = true;
            }
        }

        private void DisableAllContainers() => _containers.ForEach(it => it.Container.SetActive(false));      
        private void EnableContainer(BattlePassRewardViewState state) => _containers.First(it => it.State == state).Container.SetActive(true);
   
        private void OnClick()
        {
            _takeReward?.Invoke();
        }
        private string RewardIcon
        {
            set => _rewardIcon.sprite = Resources.Load<Sprite>(value);
        }  
        private string Background
        {
            set => _background.sprite = Resources.Load<Sprite>(value);
        }

        private string RewardCount
        {
            set => _rewardCount.text = value;
        }
        public Vector2 DroppingLootPosition => _rewardIcon.transform.position;
        private Button Button => _button ??= GetComponent<Button>();
        private TutorialUiElement TutorialUiElement => _tutorialUiElement ??= GetComponent<TutorialUiElement>();
    }
}