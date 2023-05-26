using UnityEngine;

namespace LegionMaster.Campaign.Location
{
    public class CampaignStageDoors : MonoBehaviour
    {
        private const string OPENED_STATE = "Opened";
        private const string CLOSED_STATE = "None";

        [SerializeField] private Animator _animator;

        public void Close()
        {
            _animator.Play(CLOSED_STATE);
        }

        public void Open()
        {
            _animator.Play(OPENED_STATE);
        }
    }
}