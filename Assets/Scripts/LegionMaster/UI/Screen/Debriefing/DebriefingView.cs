using LegionMaster.Extension;
using LegionMaster.Location.Session.Model;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using Zenject;

namespace LegionMaster.UI.Screen.Debriefing
{
    public class DebriefingView : MonoBehaviour
    {
        [SerializeField]
        private RewardItemPanel _rewardItemPrefab;
        [SerializeField]
        private Transform _rewardItemRoot;
        [SerializeField]
        private GameObject _winPanel;
        [SerializeField]
        private GameObject _losePanel;
        [SerializeField] 
        private ParticleSystem _winVfx;
        [Inject]
        private DiContainer _container;

        public void Init(DebriefingScreenModel debriefingScreenModel)
        {
            RemoveAllCreatedObjects();
            debriefingScreenModel.RewardsItemModels.ForEach(it => {
                var rewardItemPanel = _container.InstantiatePrefabForComponent<RewardItemPanel>(_rewardItemPrefab, _rewardItemRoot);
                rewardItemPanel.Init(it);
            });
            WinPanelEnabled = debriefingScreenModel.BattleResult == BattleResult.WIN;
        }

        private bool WinPanelEnabled
        {
            set
            {
                _winPanel.SetActive(value);
                _losePanel.SetActive(!value);
                if (value) _winVfx.Play();
            }
        }

        public void OnDisable()
        {
            RemoveAllCreatedObjects();
        }

        private void RemoveAllCreatedObjects()
        {
            _rewardItemRoot.DestroyAllChildren();
        }
    }
}